using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ZoneSimulator
{
    /// <summary>单条 TCP 连接的模拟会话：应答上位机指令、自动推进老化流程、支持手动注入事件</summary>
    public sealed class ZoneSim : IDisposable
    {
        public const byte CodeMainBoard = 0;
        public const byte CodeBaseBoard = 16;
        public const byte CodeDigitalBoard = 32;
        public const byte CodeAnalogBoard = 33;
        public const byte CodePowerBoard = 48;
        public const byte CodeTempControlBoard = 64;
        public const byte CodeBurnInBoard = 234;

        private const int DpsCount = 24;
        private const int VihCount = 2;
        private const int ThermostatCount = 4;

        private readonly object _sendLock = new();
        private readonly FrameParser _parser = new();
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private readonly List<DpsSim> _dps = new();
        private readonly List<DpsSim> _vih = new();
        private readonly CancellationTokenSource _cts = new();
        private readonly PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(500));
        private CancellationTokenSource _seqCts;
        private int _tickCount;
        private bool _disposed;
        private bool _hasControlCommand;

        public ZoneSim(int id, TcpClient client)
        {
            Id = id;
            _client = client;
            _client.NoDelay = true;
            _stream = client.GetStream();
            Remote = client.Client.RemoteEndPoint?.ToString() ?? "?";

            for (byte i = 0; i < DpsCount; i++)
            {
                _dps.Add(new DpsSim(i, DpsType.DPS));
            }

            for (byte i = 0; i < VihCount; i++)
            {
                _vih.Add(new DpsSim(i, DpsType.VIH) { OutputVoltage = 1800 });
            }

            _parser.FrameReceived += Handle;
        }

        public int Id { get; }
        public string Remote { get; }

        public bool Connected
        {
            get
            {
                try
                {
                    return _client.Connected;
                }
                catch
                {
                    return false;
                }
            }
        }

        public ZoneState State { get; private set; } = ZoneState.Idle;
        public bool Plugged { get; private set; }

        /// <summary>收到“开始老化”后是否自动推进流程</summary>
        public bool Auto { get; set; } = true;

        /// <summary>自动模式下每 500ms 推进的秒数</summary>
        public int Speed { get; set; } = 5;

        /// <summary>单次老化总时长（秒）</summary>
        public int Total { get; set; } = 600;

        public int Cost { get; private set; }
        public byte StepId { get; set; }
        public byte VectorId { get; set; }

        /// <summary>温控目标温度 0.1℃</summary>
        public int ThermostatTarget { get; set; } = 850;

        /// <summary>温控实际温度 0.1℃</summary>
        public int ThermostatActual { get; private set; } = 250;

        public bool Verbose { get; set; }

        public event Action<ZoneSim> Closed;
        public event Action<string> Logged;

        public IReadOnlyList<DpsSim> Dpss => _dps;
        public IReadOnlyList<DpsSim> Vihs => _vih;

        #region 生命周期

        public void Start()
        {
            _ = ReceiveLoopAsync();
            _ = TickLoopAsync();
            _ = GreetAsync();
        }

        private async Task GreetAsync()
        {
            await Task.Delay(300).ConfigureAwait(false);
            if (_disposed || _hasControlCommand)
            {
                return;
            }

            SetPlugged(false);
            SetState(ZoneState.Idle);
            SendDpsState();
        }

        private async Task ReceiveLoopAsync()
        {
            byte[] buffer = new byte[64 * 1024];
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    int count = await _stream.ReadAsync(buffer, 0, buffer.Length, _cts.Token).ConfigureAwait(false);
                    if (count <= 0)
                    {
                        break;
                    }

                    _parser.Append(buffer[..count]);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Log($"连接异常：{ex.Message}");
            }
            finally
            {
                Log("上位机已断开");
                Closed?.Invoke(this);
                Dispose();
            }
        }

        private async Task TickLoopAsync()
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(_cts.Token).ConfigureAwait(false))
                {
                    _tickCount++;
                    try
                    {
                        Tick();
                    }
                    catch (Exception ex)
                    {
                        Log("周期任务异常：" + ex.Message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void Tick()
        {
            bool powered = State is ZoneState.PowerUp or ZoneState.Burning;

            if (powered)
            {
                RampPower();
            }

            if (Auto && State == ZoneState.Burning)
            {
                Cost += Speed;
                if (Cost >= Total)
                {
                    Cost = Total;
                    SendBurnState();
                    Log($"老化完成（{Total}s），上报 Stop");
                    SetState(ZoneState.Stop);
                    PowerOff();
                }
                else
                {
                    SendBurnState();
                }
            }

            if (State == ZoneState.Burning && _tickCount % 2 == 0)
            {
                SendThermostat();
            }
            else if (!powered && _tickCount % 6 == 0 && ThermostatActual > 250)
            {
                ThermostatActual -= 5;
            }
        }

        #endregion

        #region 上位机指令处理

        private void Handle(ZoneFunctionCode code, byte[] payload, uint crc)
        {
            int length = payload?.Length ?? 0;

            if (Verbose && code != ZoneFunctionCode.DpsState)
            {
                Log($"← {code}(0x{(byte)code:X2}) {length}B crc=0x{crc:X8}");
            }

            switch (code)
            {
                case ZoneFunctionCode.Version:
                    Log("← 查询版本信息");
                    SendBoards();
                    break;

                case ZoneFunctionCode.DpsState:
                    SendDpsState();
                    break;

                case ZoneFunctionCode.ControlAndState:
                    _hasControlCommand = true;
                    if (length > 0 && payload[0] == 0x01)
                    {
                        Log("← 开始老化");
                        StartBurn();
                    }
                    else
                    {
                        Log("← 停止老化");
                        StopBurn();
                    }

                    break;

                case ZoneFunctionCode.Trigger:
                    {
                        TriggerOutputType type = length > 0 ? (TriggerOutputType)payload[0] : TriggerOutputType.Close;
                        byte pin = length > 1 ? payload[1] : (byte)0;
                        Log($"← 设置 Trigger：{type} pin={pin}");
                        Send(ZoneFunctionCode.Trigger, Payloads.TriggerReport(type, pin));
                        break;
                    }

                case ZoneFunctionCode.Upgrade:
                    Log($"← 固件下发 {length:N0} 字节，模拟升级过程");
                    SimulateUpgrade();
                    break;

                case ZoneFunctionCode.DeviceCheck:
                    Log("← 启动设备自检");
                    SendDeviceCheck(false);
                    break;

                case ZoneFunctionCode.DpsCheck:
                    Log("← 启动 DPS 自检脚本");
                    SimulateDpsCheck(payload, false);
                    break;

                case ZoneFunctionCode.ThermostatPID:
                    ParseAndEchoPid(payload);
                    break;

                case ZoneFunctionCode.Thermostat:
                    ParseThermostatConfig(payload);
                    break;

                case ZoneFunctionCode.ConfigFinish:
                    Log($"← 器件配置完成（{(length > 0 ? payload[0] : 0)}）");
                    break;

                case ZoneFunctionCode.PatternCount:
                case ZoneFunctionCode.Pattern:
                case ZoneFunctionCode.RegisterCount:
                case ZoneFunctionCode.Register:
                case ZoneFunctionCode.DpsPlanCount:
                case ZoneFunctionCode.DpsPlan:
                case ZoneFunctionCode.UpSet:
                case ZoneFunctionCode.StepSet:
                case ZoneFunctionCode.DownSet:
                    Log($"← {code} 数据 {length:N0} 字节");
                    break;

                default:
                    Log($"← 未处理的功能码 {code}(0x{(byte)code:X2}) {length}B");
                    break;
            }
        }

        private void ParseAndEchoPid(byte[] payload)
        {
            if (payload is null || payload.Length < 1)
            {
                return;
            }

            int count = payload[0];
            int index = 1;
            List<ThermostatPid> pids = new();

            for (int i = 0; i < count && index + 13 <= payload.Length; i++)
            {
                byte id = payload[index++];
                int kp = ReadI32(payload, index);
                index += 4;
                int ki = ReadI32(payload, index);
                index += 4;
                int kd = ReadI32(payload, index);
                index += 4;
                pids.Add(new ThermostatPid(id, kp, ki, kd));
                Log($"← 温控 PID 设置：id={id} Kp={kp} Ki={ki} Kd={kd}");
            }

            if (pids.Count > 0)
            {
                Send(ZoneFunctionCode.ThermostatPID, Payloads.ThermostatPids(pids));
            }
        }

        private void ParseThermostatConfig(byte[] payload)
        {
            if (payload is null || payload.Length < 1)
            {
                return;
            }

            int count = payload[0];
            int index = 1;

            for (int i = 0; i < count && index + 6 <= payload.Length; i++)
            {
                byte id = payload[index++];
                bool enable = payload[index++] != 0;
                int temperature = ReadI32(payload, index);
                index += 4;

                if (enable)
                {
                    ThermostatTarget = temperature;
                }

                Log($"← 温控设置：id={id} {(enable ? "开启" : "关闭")} 目标 {(temperature / 10.0):F1}℃");
            }

            SendThermostat();
        }

        #endregion

        #region 上报

        public void SetState(ZoneState state)
        {
            State = state;
            Send(ZoneFunctionCode.ControlAndState, Payloads.State(state, LoadMode.Default));
            Log($"→ 状态上报：{state}");
        }

        public void SetError(ZoneErrorType type, byte[] detail8)
        {
            State = ZoneState.Error;
            Send(ZoneFunctionCode.ControlAndState, Payloads.StateError(type, LoadMode.Default, detail8));
            Log($"→ 状态上报：Error({type})");
        }

        public void SetPlugged(bool plugged)
        {
            Plugged = plugged;
            Send(ZoneFunctionCode.BurnInBoardInsertState, new[] { (byte)(plugged ? 0x03 : 0x00) });
            Log($"→ 插板状态：{(plugged ? "已插板" : "未插板")}");
        }

        public void SendDpsState()
        {
            Send(ZoneFunctionCode.DpsState, Payloads.DpsStateReport(_dps, _vih));
        }

        public void SendCurrentPlan(byte planId)
        {
            Send(ZoneFunctionCode.CurrentDpsPlan, Payloads.CurrentDpsPlan(planId));
            Log($"→ 当前电源方案：{(planId == 0xFF ? "无" : planId.ToString())}");
        }

        public void SendBurnState()
        {
            Send(ZoneFunctionCode.BurnState, Payloads.BurnState(
                Total, StepId, ExecuteMode.Time, Total, Cost,
                true, 0, 0, ExecuteMode.Time, Total, Cost % Math.Max(Total, 1), VectorId));
        }

        public void SendCompareErrors(int count, byte stepId, byte vectorId)
        {
            List<CompareErrorItem> items = new();
            Random random = new(Environment.TickCount);

            for (int i = 0; i < count; i++)
            {
                items.Add(new CompareErrorItem(
                    (byte)random.Next(0, 4),
                    random.Next(1, 100000),
                    random.Next(0, 256)));
            }

            Send(ZoneFunctionCode.CompareError, Payloads.CompareErrors(stepId, vectorId, items));
            Log($"→ 回检报错 {count} 条（step={stepId} vector={vectorId}）");
        }

        public void SendThermostat()
        {
            if (State == ZoneState.Burning)
            {
                if (ThermostatActual < ThermostatTarget)
                {
                    ThermostatActual = Math.Min(ThermostatTarget, ThermostatActual + 20);
                }
                else if (ThermostatActual > ThermostatTarget)
                {
                    ThermostatActual = Math.Max(ThermostatTarget, ThermostatActual - 10);
                }
            }

            bool enable = State is ZoneState.PowerUp or ZoneState.Burning;
            List<ThermostatReport> items = new();

            for (byte id = 0; id < ThermostatCount; id++)
            {
                items.Add(new ThermostatReport(id, enable, ThermostatActual));
            }

            Send(ZoneFunctionCode.Thermostat, Payloads.Thermostats(items));
        }

        public void SendBoards()
        {
            Send(ZoneFunctionCode.Version, Payloads.Board(CodeMainBoard, 0, 0xFF, "MB-LSIC12K-01", "MB2026103001", 1,
                new FirmwareEntry(FirmwareType.FPGA, 0x00010500), new FirmwareEntry(FirmwareType.ARM, 0x00020301)));
            Send(ZoneFunctionCode.Version, Payloads.Board(CodeBaseBoard, 1, 0, "BB-LSIC12K-01", "BB2026103001", 1,
                new FirmwareEntry(FirmwareType.FPGA, 0x00010100)));
            Send(ZoneFunctionCode.Version, Payloads.Board(CodeDigitalBoard, 2, 0, "DB-LSIC12K-01", "DB2026103001", 1,
                new FirmwareEntry(FirmwareType.FPGA, 0x00010200)));
            Send(ZoneFunctionCode.Version, Payloads.Board(CodeAnalogBoard, 3, 0, "AB-LSIC12K-01", "AB2026103001", 1,
                new FirmwareEntry(FirmwareType.FPGA, 0x00010200)));
            Send(ZoneFunctionCode.Version, Payloads.Board(CodePowerBoard, 4, 0, "PB-LSIC12K-01", "PB2026103001", 1,
                new FirmwareEntry(FirmwareType.ARM, 0x00010400)));
            Send(ZoneFunctionCode.Version, Payloads.Board(CodeTempControlBoard, 5, 0, "TB-LSIC12K-01", "TB2026103001", 1,
                new FirmwareEntry(FirmwareType.ARM, 0x00010100)));
            Send(ZoneFunctionCode.Version, Payloads.Board(CodeBurnInBoard, 6, 0, "BB-LSIC12K-BI", "BI2026103001", 1,
                new FirmwareEntry(FirmwareType.FPGA, 0x00010300)));

            Log("→ 上报板卡版本 7 块");
        }

        public void SendDeviceCheck(bool withFail)
        {
            Send(ZoneFunctionCode.DeviceCheck, Payloads.DeviceCheckItem(0x00, 0x01, 0x00, 0x00));
            Send(ZoneFunctionCode.DeviceCheck, Payloads.DeviceCheckItem(0x00, 0x02, 0x00, 0x00));
            Send(ZoneFunctionCode.DeviceCheck, Payloads.DeviceCheckItem(0x20, 0x01, 0x00, 0x00));
            Send(ZoneFunctionCode.DeviceCheck, Payloads.DeviceCheckItem(0x30, 0x01, 0x00, 0x00));

            if (withFail)
            {
                Send(ZoneFunctionCode.DeviceCheck, Payloads.DeviceCheckItem(0x30, 0x02, 0x01, 0x05));
            }

            Log($"→ 设备自检 {(withFail ? "含失败项" : "全部通过")}");
        }

        public void SimulateDpsCheck(byte[] script, bool withFail)
        {
            int count = 8;
            if (script is not null && script.Length >= 4)
            {
                count = Math.Min(ReadI32(script, 0), DpsCount);
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    SetState(ZoneState.DpsChecking);
                    await Task.Delay(300).ConfigureAwait(false);

                    for (int round = 1; round <= 2; round++)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            DpsSim dps = _dps[i];
                            DpsState state = withFail && i == 0 && round == 2 ? DpsState.OutputOv : DpsState.Runing;

                            dps.OutputVoltage = 1000 + i * 100;
                            dps.OutputCurrent = 800;
                            dps.State = state;

                            Send(ZoneFunctionCode.DpsCheck,
                                Payloads.DpsCheckResult(round * count - count + i + 1, count * 2, round, 2, dps, state));

                            await Task.Delay(120).ConfigureAwait(false);
                        }
                    }

                    SetState(ZoneState.Idle);
                    Log($"→ DPS 自检完成（{count} 路 × 2 轮）{(withFail ? "，含 1 路故障" : "，全部通过")}");
                }
                catch (Exception ex)
                {
                    Log("DPS 自检模拟异常：" + ex.Message);
                }
            });
        }

        public void SimulateUpgrade()
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    SetState(ZoneState.Upgrading);
                    await Task.Delay(200).ConfigureAwait(false);

                    for (int progress = 0; progress <= 100; progress += 10)
                    {
                        Send(ZoneFunctionCode.Upgrade,
                            Payloads.UpgradeProgress(CodeMainBoard, 0, (byte)progress, UpgradeState.Updating));
                        await Task.Delay(120).ConfigureAwait(false);
                    }

                    Send(ZoneFunctionCode.Upgrade,
                        Payloads.UpgradeProgress(CodeMainBoard, 0, 100, UpgradeState.Finish));
                    Send(ZoneFunctionCode.UpgradeFinish, Array.Empty<byte>());
                    SetState(ZoneState.Idle);
                    Log("→ 升级完成");
                }
                catch (Exception ex)
                {
                    Log("升级模拟异常：" + ex.Message);
                }
            });
        }

        #endregion

        #region 老化流程

        public void StartBurn()
        {
            _seqCts?.Cancel();
            _seqCts = new CancellationTokenSource();
            _ = RunStartAsync(_seqCts.Token);
        }

        public void StopBurn()
        {
            _seqCts?.Cancel();
            _seqCts = new CancellationTokenSource();
            _ = RunStopAsync(_seqCts.Token);
        }

        private async Task RunStartAsync(CancellationToken token)
        {
            SetState(ZoneState.Ready);
            await DelayAsync(300, token).ConfigureAwait(false);
            if (token.IsCancellationRequested)
            {
                return;
            }

            SetState(ZoneState.PowerUp);
            PowerOn();
            await DelayAsync(700, token).ConfigureAwait(false);
            if (token.IsCancellationRequested)
            {
                return;
            }

            Cost = 0;
            SetState(ZoneState.Burning);
            SendCurrentPlan(0);
            SendBurnState();
        }

        private async Task RunStopAsync(CancellationToken token)
        {
            SetState(ZoneState.PowerDown);
            PowerOff();
            await DelayAsync(400, token).ConfigureAwait(false);
            if (token.IsCancellationRequested)
            {
                return;
            }

            SetState(ZoneState.Stop);
        }

        private void PowerOn()
        {
            for (int i = 0; i < _dps.Count; i++)
            {
                DpsSim dps = _dps[i];
                dps.TargetVoltage = 1000 + (i % 4) * 500;
                dps.TargetCurrent = 500 + (i % 3) * 200;
                dps.State = DpsState.Runing;
            }

            foreach (DpsSim vih in _vih)
            {
                vih.TargetVoltage = 1800;
                vih.TargetCurrent = 50;
                vih.State = DpsState.Runing;
            }
        }

        private void PowerOff()
        {
            foreach (DpsSim dps in _dps.Concat(_vih))
            {
                dps.TargetVoltage = 0;
                dps.TargetCurrent = 0;
                dps.OutputVoltage = 0;
                dps.OutputCurrent = 0;
                dps.State = DpsState.Off;
            }
        }

        private void RampPower()
        {
            foreach (DpsSim dps in _dps)
            {
                dps.OutputVoltage = Ramp(dps.OutputVoltage, dps.TargetVoltage, 200);
                dps.OutputCurrent = Ramp(dps.OutputCurrent, dps.TargetCurrent, 100);
                dps.Temperature = Ramp(dps.Temperature, 450, 10);
            }

            foreach (DpsSim vih in _vih)
            {
                vih.OutputVoltage = Ramp(vih.OutputVoltage, vih.TargetVoltage, 100);
                vih.OutputCurrent = Ramp(vih.OutputCurrent, vih.TargetCurrent, 10);
            }
        }

        private static int Ramp(int current, int target, int step)
        {
            if (current < target)
            {
                return Math.Min(target, current + step);
            }

            if (current > target)
            {
                return Math.Max(target, current - step);
            }

            return current;
        }

        #endregion

        #region 手动注入

        public void SetDps(byte id, int? voltage, int? current, int? temperature, DpsState? state)
        {
            DpsSim dps = _dps.Find(d => d.Id == id) ?? _vih.Find(d => d.Id == id);
            if (dps is null)
            {
                Log($"没有 id={id} 的电源");
                return;
            }

            if (voltage.HasValue)
            {
                dps.OutputVoltage = voltage.Value;
                dps.TargetVoltage = voltage.Value;
            }

            if (current.HasValue)
            {
                dps.OutputCurrent = current.Value;
                dps.TargetCurrent = current.Value;
            }

            if (temperature.HasValue)
            {
                dps.Temperature = temperature.Value;
            }

            if (state.HasValue)
            {
                dps.State = state.Value;
            }

            SendDpsState();
            Log($"→ {dps.Name} 输出 {(dps.OutputVoltage / 1000.0):F3}V {(dps.OutputCurrent / 1000.0):F3}A {(dps.Temperature / 10.0):F1}℃ {dps.State}");
        }

        public void SetAllDps(bool on)
        {
            if (on)
            {
                PowerOn();
                foreach (DpsSim dps in _dps.Concat(_vih))
                {
                    dps.OutputVoltage = dps.TargetVoltage;
                    dps.OutputCurrent = dps.TargetCurrent;
                }
            }
            else
            {
                PowerOff();
            }

            SendDpsState();
            Log($"→ 全部电源 {(on ? "上电" : "下电")}");
        }

        public void SetBurn(int cost, int? total, byte? step, byte? vector)
        {
            if (total.HasValue)
            {
                Total = total.Value;
            }

            if (step.HasValue)
            {
                StepId = step.Value;
            }

            if (vector.HasValue)
            {
                VectorId = vector.Value;
            }

            Cost = Math.Max(0, cost);
            SendBurnState();
            Log($"→ 老化进度 {Cost}/{Total}s step={StepId} vector={VectorId}");
        }

        #endregion

        #region 基础设施

        public void Send(ZoneFunctionCode code, byte[] payload)
        {
            if (_disposed)
            {
                return;
            }

            byte[] frame = FrameProtocol.BuildFrame(code, payload);

            lock (_sendLock)
            {
                try
                {
                    _stream.Write(frame, 0, frame.Length);
                    _stream.Flush();
                }
                catch (Exception ex)
                {
                    Log($"发送失败：{ex.Message}");
                }
            }
        }

        public void Log(string message)
        {
            string line = $"[Z{Id}] {DateTime.Now:HH:mm:ss.fff} {message}";
            Console.WriteLine(line);
            Logged?.Invoke(line);
        }

        private static int ReadI32(byte[] data, int index)
        {
            return (data[index] << 24) | (data[index + 1] << 16) | (data[index + 2] << 8) | data[index + 3];
        }

        private static async Task DelayAsync(int milliseconds, CancellationToken token)
        {
            try
            {
                await Task.Delay(milliseconds, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            try
            {
                _cts.Cancel();
            }
            catch
            {
            }

            try
            {
                _seqCts?.Cancel();
            }
            catch
            {
            }

            try
            {
                _stream?.Dispose();
            }
            catch
            {
            }

            try
            {
                _client?.Close();
            }
            catch
            {
            }

            _timer.Dispose();
        }

        #endregion
    }
}
