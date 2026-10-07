using ATiS.Sim.Log;
using LSIC.App.Common.Enums;

namespace LSIC.App.Models
{
    public class Zone : ObservableObject, IDisposable
    {
        public Zone(ZoneInfo info)
        {
            Info = info ?? throw new ArgumentNullException(nameof(info));
        }
        public ZoneInfo Info { get; }
        public int Id => Info.Id;
        public string Ip => Info.Ip;
        public ushort Port => Info.Port;

        #region 老化相关
        private BurnInfo _burnInfo;
        public BurnInfo BurnInfo
        {
            get => _burnInfo;
            set => SetProperty(ref _burnInfo, value);
        }

        #endregion





        private TcpClient _client;
        private List<byte> _buffer;
        private RollingFileLogger _logger;
        private bool _disposed;
        private bool _connected;
        public bool Connected
        {
            get => _connected;
            private set => SetProperty(ref _connected, value);
        }
        private ZoneState _state = ZoneState.Offline;
        public ZoneState State
        {
            get => _state;
            private set => SetProperty(ref _state, value);
        }
        private int? _currentDpsGroupIndex;
        public int? CurrentDpsGroupIndex
        {
            get => _currentDpsGroupIndex;
            private set => SetProperty(ref _currentDpsGroupIndex, value);
        }
        private DpsRunInfo[] _dpss;
        public DpsRunInfo[] Dpss => _dpss ??= Device.Instance.SystemConfig.DpsInfos
            .Select(info => new DpsRunInfo(info)).ToArray();
        public void Startup()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(Zone));
            }

            if (_client != null)
            {
                Unsubscribe(_client);
                _client.Dispose();
            }

            _logger ??= new RollingFileLogger($@".\Log\Zone\Zone {Id}\Flow_{{date:yyyy-MM-dd}}.log",
                RollingInterval.Day, 256L * 1024 * 1024, LogLevel.Debug);

            _client = new TcpClient(Ip, Port) { CheckInterval = 3000 };
            _client.StateChanged += OnStateChanged;
            _client.ReceivedData += OnReceivedData;
            _client.ThrownException += OnClientException;
            _client.Start();
            _client.StartAutoReceive();
        }
        private Task OnStateChanged(object sender, SocketStateChangedArgs e)
        {
            Connected = e.Connected;
            _buffer = e.Connected ? new List<byte>(1024) : null;

            if (e.Connected)
            {
                State = ZoneState.Idle;
                CurrentDpsGroupIndex = null;
                _logger?.Info($"Zone {Id} connected.");
            }
            else
            {
                State = ZoneState.Offline;
                CurrentDpsGroupIndex = null;
                foreach (DpsRunInfo dps in Dpss)
                {
                    dps.Reset();
                }
                _logger?.Info($"Zone {Id} disconnected.");
            }

            return Task.CompletedTask;
        }
        private async Task OnReceivedData(object sender, SocketDataReceivedArgs e)
        {
            if (_buffer == null)
            {
                return;
            }

            _buffer.AddRange(e.Data);
            while (ZoneFrame.TryExtractFrame(_buffer, out byte code, out byte[] frameData))
            {
                try
                {
                    await AnalysisFrame((ZoneFunctionCode)code, frameData);
                }
                catch (Exception ex)
                {
                    _logger?.Error($"Zone {Id} analysis frame 0x{code:X2} error: {ex.Message}");
                }
            }
        }
        private Task OnClientException(object sender, SocketExceptionArgs e)
        {
            _logger?.Error($"Zone {Id} communication error: {e.Exception.Message}");
            return Task.CompletedTask;
        }
        private Task AnalysisFrame(ZoneFunctionCode code, byte[] data)
        {
            switch (code)
            {
                case ZoneFunctionCode.DpsState:
                    ReadDpsState(data);
                    break;
                case ZoneFunctionCode.CurrentDpsGroup:
                    int id = data[0];
                    if (id != 0xFF)
                    {
                        CurrentDpsGroupIndex = id;
                    }
                    break;
            }
            return Task.CompletedTask;
        }
        private void ReadDpsState(byte[] data)
        {
            int index = 0;
            int groupIndex = data[index++];
            if (groupIndex != 0xFF)
            {
                CurrentDpsGroupIndex = groupIndex;
            }

            int count = data[index++];
            for (int i = 0; i < count; i++)
            {
                int id = data[index++];
                DpsType type = (DpsType)data[index++];
                int inputVoltage = data.ToInt32(index, Endianness.Big); index += 4;
                int outputVoltage = data.ToInt32(index, Endianness.Big); index += 4;
                int outputCurrent = data.ToInt32(index, Endianness.Big); index += 4;
                int temperature = data.ToInt32(index, Endianness.Big); index += 4;
                DpsState state = (DpsState)data[index++];

                if (type != DpsType.Dps)
                {
                    continue; // VIH 条目字段已读，暂不处理
                }

                DpsRunInfo dps = Dpss.FirstOrDefault(j => j.Info.Id == id);
                if (dps == null)
                {
                    continue;
                }

                int symbol = dps.Info.Reverse ? -1 : 1;
                dps.Update(state, inputVoltage * symbol, outputVoltage * symbol, outputCurrent, temperature);
            }
        }

        /// <summary>
        /// 下发完整老化配置：电源组 → 向量文件 → 寄存器 → 上电 → 步次 → 下电 → 完成
        /// </summary>
        public async Task SendIcAsync()
        {
            BurnInfo info = BurnInfo;
            if (info == null)
            {
                throw new InvalidOperationException("尚未进入老化，无法下发配置");
            }

            await SendDpsPlans(info);
            await SendPatterns(info);
            await SendRegisters(info);
            await SendPowerAsync(ZoneFunctionCode.UpSet, info.Steps[0]);
            await SendSteps(info);
            await SendPowerAsync(ZoneFunctionCode.DownSet, info.Steps[info.Steps.Length - 1]);
            await SendAsync(ZoneFunctionCode.ConfigFinish);
        }

        /// <summary>
        /// ① 电源组：先数量，再逐组下发
        /// </summary>
        private async Task SendDpsPlans(BurnInfo info)
        {
            await SendAsync(ZoneFunctionCode.DpsGroupCount, (byte)info.DpsPlans.Length);

            foreach (BurnDpsPlan plan in info.DpsPlans)
            {
                byte[] data = BuildDpsGroup(plan);
                await SendAsync(ZoneFunctionCode.DpsGroup, data);
            }
        }

        /// <summary>
        /// ② 向量文件：先数量，再逐条读取下发
        /// </summary>
        private async Task SendPatterns(BurnInfo info)
        {
            await SendAsync(ZoneFunctionCode.PatternCount, (byte)info.PatternIndexByPath.Count);

            foreach (KeyValuePair<string, byte> pair in info.PatternIndexByPath)
            {
                byte[] file = File.ReadAllBytes(pair.Key);
                byte[] data = new byte[file.Length + 1];
                data[0] = pair.Value;
                Array.Copy(file, 0, data, 1, file.Length);
                await SendAsync(ZoneFunctionCode.Pattern, data);
            }
        }

        /// <summary>
        /// ③ 寄存器文件：先数量，再解析、序列化下发
        /// </summary>
        private async Task SendRegisters(BurnInfo info)
        {
            await SendAsync(ZoneFunctionCode.RegisterCount, (byte)info.RegisterIndexByPath.Count);

            foreach (KeyValuePair<string, byte> pair in info.RegisterIndexByPath)
            {
                Register register = RegisterParser.Parse(pair.Key);
                byte[] body = register.Serialize(out int count);
                byte[] data = new byte[body.Length + 2];
                data[0] = pair.Value;
                data[1] = (byte)count;
                Array.Copy(body, 0, data, 2, body.Length);
                await SendAsync(ZoneFunctionCode.Register, data);
            }
        }

        /// <summary>
        /// ④/⑥ 上电或下电设置（对应 Process 首尾的 StepPower）
        /// </summary>
        private async Task SendPowerAsync(ZoneFunctionCode code, BurnStep step)
        {
            byte[] data = new byte[8];
            data[0] = 0x01;
            data[1] = step.DpsPlanIndex;
            data[2] = 0xFF;
            data[3] = 0xFF;
            byte[] timeout = step.Timeout.ToBytes(Endianness.Big);
            Array.Copy(timeout, 0, data, 4, 4);
            await SendAsync(code, data);
        }

        /// <summary>
        /// ⑤ 步次：只下发向量步（上下电已由 UpSet/DownSet 配置）
        /// </summary>
        private async Task SendSteps(BurnInfo info)
        {
            foreach (BurnStep step in info.Steps)
            {
                if (step.Type == StepItemType.Vector)
                {
                    await SendAsync(ZoneFunctionCode.StepSet, BuildStepFrame(step));
                }
            }
        }

        /// <summary>
        /// 组装单步帧（对照 9000 SetStep）
        /// </summary>
        private static byte[] BuildStepFrame(BurnStep step)
        {
            using MemoryStream stream = new MemoryStream();

            stream.WriteByte(step.Id);
            stream.WriteByte((byte)step.Strategy.Mode);
            Write(stream, step.Strategy.TotalSecondsOrCount);
            stream.WriteByte(step.DpsPlanIndex);

            if (step.LoopEnable)
            {
                stream.WriteByte(0x01);
                stream.WriteByte(step.LoopTargetIndex);
                stream.WriteByte((byte)step.LoopStrategy.Mode);
                Write(stream, step.LoopStrategy.TotalSecondsOrCount);
            }
            else
            {
                stream.WriteByte(0x00);
                stream.WriteByte(0xFF);
                stream.WriteByte(0x00);
                Write(stream, 0);
            }

            stream.WriteByte((byte)step.Vectors.Length);
            foreach (BurnVector vector in step.Vectors)
            {
                stream.WriteByte(vector.PatternIndex);
                stream.WriteByte(vector.RegisterIndex);
            }

            return stream.ToArray();
        }

        private static byte[] BuildDpsGroup(BurnDpsPlan plan)
        {
            using MemoryStream stream = new MemoryStream();
            stream.WriteByte(plan.Id);
            stream.WriteByte((byte)plan.Dpss.Length);

            foreach (BurnDps dps in plan.Dpss)
            {
                stream.WriteByte((byte)dps.Id);
                Write(stream, dps.Vol);
                Write(stream, dps.Uvp);
                Write(stream, dps.Ovp);
                Write(stream, dps.Ocp);
                Write(stream, int.MaxValue);
                Write(stream, dps.Delay);
                Write(stream, dps.Raise);
            }

            return stream.ToArray();
        }

        private static void Write(Stream stream, int value)
        {
            byte[] bytes = value.ToBytes(Endianness.Big);
            stream.Write(bytes, 0, bytes.Length);
        }
        private async Task SendAsync(ZoneFunctionCode code, params byte[] data)
        {
            if (!Connected || _client == null)
            {
                return;
            }

            byte[] frame = ZoneFrame.Build((byte)code, data);
            await _client.SendAsync(frame);
            _logger?.Debug($"Tx: {code} 0x{(byte)code:X2} {data.ToHex(" ")}");
        }
        private void Unsubscribe(TcpClient client)
        {
            client.StateChanged -= OnStateChanged;
            client.ReceivedData -= OnReceivedData;
            client.ThrownException -= OnClientException;
        }
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;

            if (_client != null)
            {
                Unsubscribe(_client);
                _client.Dispose();
                _client = null;
            }
            _logger?.Dispose();
            _logger = null;
            _buffer = null;
        }
    }
}
