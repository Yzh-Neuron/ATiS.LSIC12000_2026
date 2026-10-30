using TcpClient = ATiS.Sim.Communication.TcpClient;

namespace LSIC.App.Models
{
    public class Zone : DeviceNodeBase, IDisposable
    {
        private const ushort FrameHeader = 0x55AA;
        private const ushort FrameEnding = 0xEAA5;
        private const int MaxPayloadLength = 4 * 1024 * 1024;
        private const int MaxErrorWritesPerTick = 128;
        private CancellationTokenSource _tickCts;
        private Task _tickTask;
        private readonly PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(500));
        private bool _disposed;
        private TcpClient _client;
        private List<byte> _buffer;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _receiveLock = new SemaphoreSlim(1, 1);
        private readonly Channel<(BurnInfo Info, CompareError Error)> _errorChannel;
        public ObservableCollection<DeviceCheckItem> DeviceCheckItems { get; } = new();
        public ZoneInfo Info { get; }
        public int Id
        {
            get => Info.Id;
        }
        public string Ip
        {
            get => Info.Ip;
        }
        public ushort Port
        {
            get => Info.Port;
        }
        private bool _connected = false;
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
        private bool _pluged;
        public bool Pluged
        {
            get => _pluged;
            private set => SetProperty(ref _pluged, value);
        }
        private TriggerOutputType _outputType;
        public TriggerOutputType OutputType
        {
            get => _outputType;
            private set => SetProperty(ref _outputType, value);
        }
        private byte? _outputPin;
        public byte? OutputPin
        {
            get => _outputPin;
            private set => SetProperty(ref _outputPin, value);
        }
        private BurnInfo _burnInfo = null;
        public BurnInfo BurnInfo
        {
            get => _burnInfo;
            private set => SetProperty(ref _burnInfo, value);
        }
        public FlowLogger Logger { get; private set; }
        public ErrorDatabase ErrorDatabase { get; private set; }
        public Dps[] Dpss { get; }
        public Vih[] Vihs { get; }
        public Zone(ZoneInfo info, Dps[] dpss)
        {
            Info = info ?? throw new ArgumentNullException(nameof(info));
            Dpss = dpss;
            Vihs = [new Vih(0), new Vih(1)];

            _errorChannel = Channel.CreateBounded<(BurnInfo, CompareError)>(new BoundedChannelOptions(4096) { SingleReader = true, FullMode = BoundedChannelFullMode.DropWrite });
            ErrorDatabase = new ErrorDatabase(this);
            Logger = new FlowLogger(this);
            FrameFilterUtility.Register<ZoneFunctionCode>(Logger, (code) => true);
        }
        public void Startup()
        {
            try
            {
                BurnInfo = SystemDatabase.Instance.GetBurnInfo(Id);

                var bib = BurnInfo?.BurnInBoard;
                if (bib != null)
                {
                    KeyValuePair<int, int>[] pairs = ErrorDatabase.GetErrors();
                    foreach (KeyValuePair<int, int> pair in pairs)
                    {
                        BurnInfo.BurnInBoard.CompareError(pair.Key, pair.Value);
                    }
                }
            }
            catch
            {
                BurnInfo = null;
            }

            if (_client is not null)
            {
                _client.StateChanged -= OnStateChanged;
                _client.ReceivedData -= OnReceivedData;
                _client.Dispose();
                _client = null;
            }

            _client = new TcpClient(Ip, Port);
            _client.StateChanged += OnStateChanged;
            _client.ReceivedData += OnReceivedData;
            _client.Start();
            _client.StartAutoReceive();

            StartTicker();
            Logger.LogInfo(LangProvider.Instance.WorkerThreadStartedMessage);
        }
        private void UpdateZoneState(ZoneState state, ZoneErrorType? error = null, byte[] data = null)
        {
            if (state == ZoneState.DeviceCheckFail)
            {
                string failMessage = string.Format(LangProvider.Instance.StateMessage, state.GetString());
                Logger.LogError(failMessage);
                PopHelper.ShowAlarm(Id + LangProvider.Instance.ZoneText, failMessage);
            }
            else if (state == ZoneState.Error)
            {
                Logger.LogError(string.Format(LangProvider.Instance.StateMessage, state.GetString()));
            }
            else if (state == ZoneState.Offline)
            {
                Logger.LogWarn(string.Format(LangProvider.Instance.StateMessage, state.GetString()));
            }
            else
            {
                Logger.LogInfo(string.Format(LangProvider.Instance.StateMessage, state.GetString()));
            }

            if (state == ZoneState.Burning)
            {
                BurnInfo?.BurnInBoard?.Burning();
            }
            else if (state == ZoneState.Stop || state == ZoneState.Error)
            {
                BurnInfo?.BurnInBoard?.Finished();
            }

            if (state == ZoneState.Error && error.HasValue && data is not null)
            {
                int index = 0;
                switch (error.Value)
                {
                    case ZoneErrorType.DPS:
                        {
                            DpsState dpsState = (DpsState)data[index++];
                            DpsType dpsType = (DpsType)data[index++];
                            int id = data[index++];
                            switch (dpsType)
                            {
                                case DpsType.DPS:
                                    {
                                        Dps dps = Dpss.FirstOrDefault(t => t.Id == id);
                                        if (dps is not null)
                                        {
                                            string message = string.Format(LangProvider.Instance.ZoneDpsErrorMessage, dps.Name, dpsState.GetString());
                                            Logger.LogError(message);
                                            PopHelper.ShowAlarm(Id + LangProvider.Instance.ZoneText, message);
                                        }
                                    }
                                    break;
                                case DpsType.VIH:
                                    {
                                        string message = string.Format(LangProvider.Instance.ZoneVIHErrorMessage, id, dpsState.GetString());
                                        Logger.LogError(message);
                                        PopHelper.ShowAlarm(Id + LangProvider.Instance.ZoneText, message);
                                    }
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        break;
                    case ZoneErrorType.Pattern:
                        {
                            PatternErrorType patternErrorType = (PatternErrorType)data[index++];
                            int stepId = data[index++];
                            int vectorStepId = data[index++];
                            string message = string.Format(LangProvider.Instance.ZonePatternErrorMessage, patternErrorType.GetString(), stepId, vectorStepId);
                            Logger.LogError(message);
                            PopHelper.ShowAlarm(Id + LangProvider.Instance.ZoneText, message);
                        }
                        break;
                    case ZoneErrorType.Device:
                        {
                            string message = string.Format(LangProvider.Instance.ZoneDeviceErrorMessage, data.ToHex());
                            Logger.LogError(message);
                            PopHelper.ShowAlarm(Id + LangProvider.Instance.ZoneText, message);
                        }
                        break;
                }
            }

            State = state;
        }

        #region 后台任务
        private void StartTicker()
        {
            _tickCts = new CancellationTokenSource();
            var token = _tickCts.Token;
            _tickTask = Task.Factory.StartNew(
                () => TickLoopAsync(token),
                token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default).Unwrap();
        }
        private void StopTicker()
        {
            _tickCts?.Cancel();

            try
            {
                _tickTask?.Wait(TimeSpan.FromSeconds(3));
            }
            catch (AggregateException ae) when (ae.InnerException is OperationCanceledException) { }
            catch (Exception ex)
            {
                Logger?.LogError($"zone {Id} stop ticker error : " + ex.Message);
            }

            _tickCts?.Dispose();
            _tickCts = null;
            _tickTask = null;

            _timer.Dispose();
        }
        private async Task TickLoopAsync(CancellationToken ct)
        {
            bool hasError = false;
            Stopwatch stopwatch = new();
            try
            {
                while (await _timer.WaitForNextTickAsync(ct).ConfigureAwait(false))
                {
                    try
                    {
                        if (Connected)
                        {
                            await GetDpsStateAsync().ConfigureAwait(false);
                        }

                        var burnInfo = BurnInfo;
                        if (State == ZoneState.Burning && burnInfo != null)
                        {
                            double intervalMs = TimeSpan.FromSeconds(burnInfo.LogInterval.TotalSeconds).TotalMilliseconds;

                            if (!stopwatch.IsRunning)
                            {
                                stopwatch.Restart();
                            }
                            else if (stopwatch.ElapsedMilliseconds >= intervalMs)
                            {
                                burnInfo.Logger.LogData(burnInfo.CurrentStep, burnInfo.CurrentVector, Dpss);
                                stopwatch.Restart();
                            }
                        }
                        else
                        {
                            stopwatch.Reset();
                        }

                        //比较错误串行入库：每轮限量清空积压
                        DrainCompareErrors();
                        hasError = false;
                    }
                    catch (Exception ex)
                    {
                        if (!hasError)
                        {
                            hasError = true;
                            string msg = ex.GetBaseException().Message;
                            Logger?.LogError(string.Format(LangProvider.Instance.ZoneBackgroundTaskErrorMessage, Id, msg));
                        }
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
        }
        private void DrainCompareErrors()
        {
            //每轮 Tick 限量串行处理积压的比较错误
            for (int i = 0; i < MaxErrorWritesPerTick && _errorChannel.Reader.TryRead(out (BurnInfo Info, CompareError Error) item); i++)
            {
                try
                {
                    var info = item.Info;
                    var error = item.Error;

                    if (ReferenceEquals(info, BurnInfo))
                    {
                        bool success = ErrorDatabase.TryAddError(error, info.ErrorLogTimeSpan);
                        if (success)
                        {
                            info.BurnInBoard.CompareError(error.Cs, error.Dr);
                            info.Logger.LogError(error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger?.LogError($"Compare error write failed: {ex.Message}");
                }
            }
        }
        #endregion

        #region 下发器件
        public async ValueTask SendIcAsync()
        {
            BurnInfo info = BurnInfo;
            if (info is null)
            {
                throw new InvalidOperationException(LangProvider.Instance.ZoneNotInBurnMessage);
            }

            info.SendIc();
            LoadMode mode = info.LoadMode;
            UpdateZoneState(ZoneState.Sending);

            Logger.LogInfo(string.Format(LangProvider.Instance.SendIcDataMessage, info.Ic.Name));

            // 发送器件开始
            await SendAsync(ZoneFunctionCode.ConfigFinish, 0x01, (byte)info.LoadMode).ConfigureAwait(false);

            // 设置电源方案
            bool setResult = await SetDpsPlanAsync(info, percentage => UpdateProcess(0d, 5d, percentage)).ConfigureAwait(false);
            if (!setResult)
            {
                return;
            }

            // 设置寄存器
            setResult = await SetRegisterAsync(info, percentage => UpdateProcess(5d, 5d, percentage)).ConfigureAwait(false);
            if (!setResult)
            {
                return;
            }

            // 设置向量
            setResult = await SetPatternAsync(info, percentage => UpdateProcess(10d, 80d, percentage)).ConfigureAwait(false);
            if (!setResult)
            {
                return;
            }

            // 设置上电方案
            setResult = await SetPowerUpAsync(info, percentage => UpdateProcess(90d, 2.5d, percentage)).ConfigureAwait(false);
            if (!setResult)
            {
                return;
            }

            // 设置步次
            setResult = await SetStepAsync(info, percentage => UpdateProcess(92.5d, 2.5d, percentage)).ConfigureAwait(false);
            if (!setResult)
            {
                return;
            }

            // 设置下电方案
            setResult = await SetPowerDownAsync(info, percentage => UpdateProcess(95d, 2.5d, percentage)).ConfigureAwait(false);
            if (!setResult)
            {
                return;
            }
            // 发送恒温器配置
            setResult = await SetThermostatAsync(info, percentage => UpdateProcess(97.5d, 2.5d, percentage)).ConfigureAwait(false);
            if (!setResult)
            {
                return;
            }
            // 发送器件结束
            await SendAsync(ZoneFunctionCode.ConfigFinish, 0x00).ConfigureAwait(false);
        }
        private void UpdateProcess(double last, double current, double percentage)
        {
            BurnInfo.SendProcess = last + Math.Round(current * percentage / 100d, 2);
        }
        private async ValueTask<bool> SetDpsPlanAsync(BurnInfo info, Action<double> progress)
        {
            try
            {
                byte length = (byte)info.DpsPlans.Length;
                await SendAsync(ZoneFunctionCode.DpsPlanCount, [length]).ConfigureAwait(false);
                foreach (BurnDpsPlan plan in info.DpsPlans)
                {
                    using MemoryStream stream = new MemoryStream();
                    stream.WriteByte(plan.Id);
                    stream.WriteByte((byte)plan.Dpss.Length);
                    foreach (BurnDps dps in plan.Dpss)
                    {
                        stream.WriteByte((byte)dps.Id);
                        stream.Write(dps.Vol.ToBytes(Endianness.Big), 0, 4);
                        stream.Write(dps.Uvp.ToBytes(Endianness.Big), 0, 4);
                        stream.Write(dps.Ovp.ToBytes(Endianness.Big), 0, 4);
                        stream.Write(dps.Ocp.ToBytes(Endianness.Big), 0, 4);
                        stream.Write((int.MaxValue).ToBytes(Endianness.Big), 0, 4);
                        stream.Write(dps.Delay.ToBytes(Endianness.Big), 0, 4);
                        stream.Write(dps.Raise.ToBytes(Endianness.Big), 0, 4);
                    }
                    await SendAsync(ZoneFunctionCode.DpsPlan, stream.ToArray()).ConfigureAwait(false);

                    progress?.Invoke((plan.Id + 1) * 100d / length);
                }

                return true;
            }
            catch (Exception e)
            {
                UpdateZoneState(ZoneState.IcError);
                Logger.LogError(string.Format(LangProvider.Instance.IcErrorMessage, [e.Message]));
                return false;
            }
        }
        private async ValueTask<bool> SetRegisterAsync(BurnInfo info, Action<double> progress)
        {
            try
            {
                byte length = (byte)info.RegisterIndexByPath.Count;
                await SendAsync(ZoneFunctionCode.RegisterCount, [length]).ConfigureAwait(false);
                foreach (KeyValuePair<string, byte> pair in info.RegisterIndexByPath)
                {
                    Register registers;
                    try
                    {
                        registers = RegisterParser.Parse(pair.Key);
                    }
                    catch (FileNotFoundException e)
                    {
                        throw new Exception(LangProvider.Instance.FileNotExistMessage + $"[ {e.FileName} ]");
                    }
                    catch (CharReaderException e)
                    {
                        throw new Exception(string.Format(LangProvider.Instance.FileParseErrorMessage, e.File, e.Line, e.Column));
                    }

                    byte[] body = registers.Serialize(out int count);
                    using MemoryStream stream = new MemoryStream();
                    stream.WriteByte(pair.Value);
                    stream.WriteByte((byte)count);
                    stream.Write(body, 0, body.Length);
                    await SendAsync(ZoneFunctionCode.Register, stream.ToArray()).ConfigureAwait(false);

                    progress?.Invoke((pair.Value + 1) * 100d / length);
                }
                return true;
            }
            catch (Exception e)
            {
                UpdateZoneState(ZoneState.IcError);
                Logger.LogError(string.Format(LangProvider.Instance.IcErrorMessage, [e.Message]));
                return false;
            }
        }
        private async ValueTask<bool> SetPatternAsync(BurnInfo info, Action<double> progress)
        {
            try
            {
                long total = 0;
                foreach (KeyValuePair<string, byte> pair in info.PatternIndexByPath)
                {
                    string file = pair.Key;
                    if (!File.Exists(file))
                    {
                        throw new Exception(LangProvider.Instance.FileNotExistMessage + $"[ {file} ]");
                    }

                    await using FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    if (stream.Length < 4)
                    {
                        throw new Exception(string.Format(LangProvider.Instance.FileLengthErrorMessage, file, 4));
                    }

                    byte[] head = new byte[4];
                    stream.ReadExactly(head, 0, head.Length);
                    int block = head.ToInt32(0, Endianness.Big);
                    if (2L * (block + 4) + 4 != stream.Length)
                    {
                        throw new Exception(string.Format(LangProvider.Instance.AbmFileNotMatchMessage, file));
                    }
                    total += stream.Length;
                }

                byte patternCount = (byte)info.PatternIndexByPath.Count;
                await SendAsync(ZoneFunctionCode.PatternCount, [patternCount]).ConfigureAwait(false);

                long sent = 0;
                foreach (KeyValuePair<string, byte> pair in info.PatternIndexByPath)
                {
                    string file = pair.Key;
                    await using FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    await SendAsync(ZoneFunctionCode.Pattern, stream, read =>
                    {
                        sent += read;
                        progress?.Invoke(sent * 100d / total);
                    }, pair.Value).ConfigureAwait(false);
                }
                return true;
            }
            catch (Exception e)
            {
                UpdateZoneState(ZoneState.IcError);
                Logger.LogError(string.Format(LangProvider.Instance.IcErrorMessage, [e.Message]));
                return false;
            }
        }
        private async ValueTask<bool> SetPowerUpAsync(BurnInfo info, Action<double> progress)
        {
            try
            {
                var burnStepPower = info.PowerOnStep;
                if (burnStepPower.Enable == false)
                {
                    await SendAsync(ZoneFunctionCode.UpSet, 0x00).ConfigureAwait(false);
                }
                else
                {
                    byte[] data = new byte[8];
                    data[0] = 0x01;
                    data[1] = burnStepPower.DpsPlanIndex;
                    data[2] = burnStepPower.Vector?.PatternIndex ?? 0xFF;
                    data[3] = burnStepPower.Vector?.RegisterIndex ?? 0xFF;
                    Array.Copy(burnStepPower.Timeout.ToBytes(Endianness.Big), 0, data, 4, 4);
                    await SendAsync(ZoneFunctionCode.UpSet, data).ConfigureAwait(false);
                }

                // 调用进度回调
                progress?.Invoke(100d);
                return true;
            }
            catch (Exception e)
            {
                UpdateZoneState(ZoneState.IcError);
                Logger.LogError(string.Format(LangProvider.Instance.IcErrorMessage, [e.Message]));
                return false;
            }
        }
        private async ValueTask<bool> SetPowerDownAsync(BurnInfo info, Action<double> progress)
        {
            try
            {
                var burnStepPower = info.PowerOffStep;
                if (burnStepPower.Enable == false)
                {
                    await SendAsync(ZoneFunctionCode.DownSet, 0x00).ConfigureAwait(false);
                }
                else
                {
                    byte[] data = new byte[8];
                    data[0] = 0x01;
                    data[1] = burnStepPower.DpsPlanIndex;
                    data[2] = burnStepPower.Vector?.PatternIndex ?? 0xFF;
                    data[3] = burnStepPower.Vector?.RegisterIndex ?? 0xFF;
                    Array.Copy(burnStepPower.Timeout.ToBytes(Endianness.Big), 0, data, 4, 4);
                    await SendAsync(ZoneFunctionCode.DownSet, data).ConfigureAwait(false);
                }

                // 调用进度回调
                progress?.Invoke(100d);
                return true;
            }
            catch (Exception e)
            {
                UpdateZoneState(ZoneState.IcError);
                Logger.LogError(string.Format(LangProvider.Instance.IcErrorMessage, [e.Message]));
                return false;
            }
        }
        private async ValueTask<bool> SetStepAsync(BurnInfo info, Action<double> progress)
        {
            try
            {
                int length = info.Steps.Length;
                for (int i = 0; i < length; i++)
                {
                    BurnStepVector step = info.Steps[i];
                    using MemoryStream stream = new MemoryStream();

                    // 基础信息 索引、运行模式、运行时间、电源方案等
                    stream.WriteByte((byte)i);
                    stream.WriteByte((byte)step.Strategy.Mode);
                    stream.Write(step.Strategy.TotalSecondsOrCount.ToBytes(Endianness.Big), 0, 4);
                    stream.WriteByte(step.DpsPlanIndex);

                    // 循环策略（仅 Start 步有，其他步写占位）
                    stream.WriteByte(Convert.ToByte(step.LoopEnable));
                    stream.WriteByte(step.LoopTargetIndex);
                    if (step.LoopEnable)
                    {
                        stream.WriteByte((byte)step.LoopStrategy.Mode);
                        stream.Write(step.LoopStrategy.TotalSecondsOrCount.ToBytes(Endianness.Big), 0, 4);
                    }
                    else
                    {
                        stream.WriteByte(0);
                        stream.Write(new byte[4], 0, 4);
                    }

                    // 子步次
                    stream.WriteByte((byte)step.Vectors.Length);
                    foreach (var vector in step.Vectors)
                    {
                        stream.WriteByte(vector.PatternIndex);
                        stream.WriteByte(vector.RegisterIndex);
                    }

                    // 发送步次数据
                    await SendAsync(ZoneFunctionCode.StepSet, stream.ToArray()).ConfigureAwait(false);

                    // 调用进度回调
                    progress?.Invoke((i + 1) * 100d / length);
                }
                return true;
            }
            catch (Exception e)
            {
                UpdateZoneState(ZoneState.IcError);
                Logger.LogError(string.Format(LangProvider.Instance.IcErrorMessage, [e.Message]));
                return false;
            }
        }
        private async ValueTask<bool> SetThermostatAsync(BurnInfo info, Action<double> progress)
        {
            try
            {
                int length = info.BurnInBoard.Thermostats.Length;
                byte[] data = new byte[length * 6 + 1];

                // 写入恒温器数量
                data[0] = (byte)length;

                // 写入恒温器配置
                for (int i = 0; i < length; i++)
                {
                    BurnThermostat setting = info.BurnInBoard.Thermostats[i];
                    data[i * 6 + 1] = (byte)setting.Id;
                    data[i * 6 + 2] = setting.Enable ? (byte)0x01 : (byte)0x00;
                    Array.Copy(setting.TargetTemperature.ToBytes(Endianness.Big), 0, data, i * 6 + 3, 4);
                }

                // 发送恒温器配置
                await SendAsync(ZoneFunctionCode.Thermostat, data).ConfigureAwait(false);

                // 调用进度回调
                progress?.Invoke(100);
                return true;
            }
            catch (Exception e)
            {
                UpdateZoneState(ZoneState.IcError);
                Logger.LogError(string.Format(LangProvider.Instance.IcErrorMessage, [e.Message]));
                return false;
            }
        }
        #endregion

        #region 老化功能
        public override async Task QueryVersionAsync()
        {
            ClearBoards();
            await SendAsync(ZoneFunctionCode.Version);
        }
        protected override Task SendUpgrade(byte[] payload)
        {
            return SendAsync(ZoneFunctionCode.Upgrade, payload);
        }
        public async ValueTask TriggerAsync(TriggerOutputType type, int dr)
        {
            byte[] data = [(byte)type, (byte)dr];
            await SendAsync(ZoneFunctionCode.Trigger, data);
        }
        public async ValueTask StartBurnAsync(byte[] dutMask)
        {
            ErrorDatabase.Clear();
            BurnInfo.StartBurn(Id, DateTime.Now, dutMask);
            await SendAsync(ZoneFunctionCode.ControlAndState, 0x01);
        }
        public async ValueTask StopBurnAsync()
        {
            await SendAsync(ZoneFunctionCode.ControlAndState, 0x00);
        }
        public async ValueTask StartDeviceCheckAsync()
        {
            this.InvokeOnUI(DeviceCheckItems.Clear);
            await SendAsync(ZoneFunctionCode.DeviceCheck);
        }
        public async ValueTask StartDpsCheckAsync(string script, DpsCommand[] commands)
        {
            Logger.LogInfo(string.Format(LangProvider.Instance.RunDpsCheckScriptMessage, script));
            using MemoryStream stream = new MemoryStream();
            stream.Write(commands.Length.ToBytes(Endianness.Big), 0, 4);
            foreach (DpsCommand command in commands)
            {
                stream.WriteByte((byte)command.Id);
                stream.WriteByte(0);
                stream.Write(command.Vol.ToBytes(Endianness.Big), 0, 4);
                stream.Write(command.Uvp.ToBytes(Endianness.Big), 0, 4);
                stream.Write(command.Ovp.ToBytes(Endianness.Big), 0, 4);
                stream.Write(command.Ocp.ToBytes(Endianness.Big), 0, 4);
                stream.Write((int.MaxValue).ToBytes(Endianness.Big), 0, 4);
                stream.Write(0.ToBytes(Endianness.Big), 0, 4);
                stream.Write(command.Raise.ToBytes(Endianness.Big), 0, 4);
                stream.Write(command.Times.ToBytes(Endianness.Big), 0, 4);
                stream.Write(command.Interval.ToBytes(Endianness.Big), 0, 4);
            }

            await SendAsync(ZoneFunctionCode.DpsCheck, stream.ToArray());
        }
        public async ValueTask GetDpsStateAsync()
        {
            await SendAsync(ZoneFunctionCode.DpsState);
        }
        public async ValueTask SendPatternFile(byte index)
        {
            try
            {
                if (BurnInfo is null)
                {
                    return;
                }

                if (index >= BurnInfo.PatternIndexByPath.Count)
                {
                    return;
                }

                string file = BurnInfo.PatternIndexByPath.FirstOrDefault(i => i.Value == index).Key;
                if (file is null)
                {
                    return;
                }

                long total = 0;

                if (!File.Exists(file))
                {
                    throw new Exception(LangProvider.Instance.FileNotExistMessage + $"[ {file} ]");
                }

                await using FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read);
                if (stream.Length < 4)
                {
                    throw new Exception(string.Format(LangProvider.Instance.FileLengthErrorMessage, file, 4));
                }

                byte[] head = new byte[4];
                stream.ReadExactly(head, 0, head.Length);
                int block = head.ToInt32(0, Endianness.Big);
                if (2L * (block + 4) + 4 != stream.Length)
                {
                    throw new Exception(string.Format(LangProvider.Instance.AbmFileNotMatchMessage, file));
                }
                total += stream.Length;

                long sent = 0;

                await SendAsync(ZoneFunctionCode.Pattern, stream, read =>
                {
                    sent += read;
                    BurnInfo.SendProcess = sent * 100d / total;
                }, index).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                UpdateZoneState(ZoneState.IcError);
                Logger.LogError(string.Format(LangProvider.Instance.IcErrorMessage, [e.Message]));
            }
        }
        public async ValueTask SendRegisterFile(byte index)
        {
            try
            {
                if (BurnInfo is null)
                {
                    return;
                }

                if (index >= BurnInfo.RegisterIndexByPath.Count)
                {
                    return;
                }

                string file = BurnInfo.RegisterIndexByPath.FirstOrDefault(i => i.Value == index).Key;
                if (file is null)
                {
                    return;
                }

                Register registers;
                try
                {
                    registers = RegisterParser.Parse(file);
                }
                catch (FileNotFoundException e)
                {
                    throw new Exception(LangProvider.Instance.FileNotExistMessage + $"[ {e.FileName} ]");
                }
                catch (CharReaderException e)
                {
                    throw new Exception(string.Format(LangProvider.Instance.FileParseErrorMessage, e.File, e.Line, e.Column));
                }

                byte[] body = registers.Serialize(out int count);
                using MemoryStream stream = new MemoryStream();
                stream.WriteByte(index);
                stream.WriteByte((byte)count);
                stream.Write(body, 0, body.Length);
                await SendAsync(ZoneFunctionCode.Register, stream.ToArray()).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                UpdateZoneState(ZoneState.IcError);
                Logger.LogError(string.Format(LangProvider.Instance.IcErrorMessage, [e.Message]));
            }
        }
        public async ValueTask SetThermostatPIDAsync(int id, double kp, double ki, double kd)
        {
            double kpScaled = Math.Round(kp * 100, MidpointRounding.AwayFromZero);
            double kiScaled = Math.Round(ki * 100, MidpointRounding.AwayFromZero);
            double kdScaled = Math.Round(kd * 100, MidpointRounding.AwayFromZero);

            kpScaled = Math.Clamp(kpScaled, int.MinValue, int.MaxValue);
            kiScaled = Math.Clamp(kiScaled, int.MinValue, int.MaxValue);
            kdScaled = Math.Clamp(kdScaled, int.MinValue, int.MaxValue);

            int kpRaw = (int)kpScaled;
            int kiRaw = (int)kiScaled;
            int kdRaw = (int)kdScaled;

            byte[] data = new byte[13];
            data[0] = (byte)id;
            Array.Copy(kpRaw.ToBytes(Endianness.Big), 0, data, 1, 4);
            Array.Copy(kiRaw.ToBytes(Endianness.Big), 0, data, 5, 4);
            Array.Copy(kdRaw.ToBytes(Endianness.Big), 0, data, 9, 4);
            await SendAsync(ZoneFunctionCode.ThermostatPID, data).ConfigureAwait(false);
        }
        public void ReloadBurnInfo(BurnInfo burnInfo)
        {
            BurnInfo?.Dispose();
            BurnInfo = burnInfo;
        }
        public void ResetStateToIdle()
        {
            BurnInfo?.Dispose();
            BurnInfo = null;
            if (State is ZoneState.Ready or ZoneState.Stop or ZoneState.IcError)
            {
                UpdateZoneState(ZoneState.Idle);
            }
            ErrorDatabase.Clear();
        }
        #endregion

        #region 通信相关
        private async Task OnStateChanged(object sender, SocketStateChangedArgs e)
        {
            try
            {
                Connected = e.Connected;
                _buffer = Connected ? new List<byte>(1024) : null;
                if (e.Connected)
                {
                    await QueryVersionAsync();
                }
                else
                {
                    UpdateZoneState(ZoneState.Offline);
                    Pluged = false;
                    this.InvokeOnUI(DeviceCheckItems.Clear);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Zone {Id} Error: {ex.Message}");
            }
        }
        private async Task OnReceivedData(object sender, SocketDataReceivedArgs e)
        {
            if (!await _receiveLock.WaitAsync(500).ConfigureAwait(false))
            {
                if (Device.Instance.Mode == DeviceMode.Administrator)
                {
                    Logger.LogDebug(string.Format(LangProvider.Instance.ReceiveBusyDropMessage, e.Data?.Length ?? 0));
                }
                return;
            }
            try
            {
                if (_buffer is null)
                {
                    return;
                }
                _buffer.AddRange(e.Data);
                while (_buffer.Count > 7)//缓冲区字节数量至少大于7个才能组成有效的报文帧格式
                {
                    //如果前两个字节无法组成有效帧头，移除第一个字节
                    ushort header = _buffer.Slice(0, 2).ToUInt16(0, Endianness.Big);
                    if (header != FrameHeader)
                    {
                        _buffer.RemoveAt(0);
                        continue;
                    }
                    //获取数据长度
                    int length = _buffer.Slice(2 + 1, 4).ToInt32(0, Endianness.Big);

                    //长度非法（负数或超出上限）时丢弃当前帧头并重同步，防止解析卡死与内存堆积
                    if (length < 0 || length > MaxPayloadLength)
                    {
                        Logger.LogError(string.Format(LangProvider.Instance.AnalysisFrameErrorMessage, string.Format(LangProvider.Instance.InvalidFrameLengthMessage, length)));
                        _buffer.RemoveAt(0);
                        continue;
                    }
                    int total = 2 + 1 + 4 + length + 4 + 2;//帧头 功能码 数据长度 数据数值 CRC校验 帧尾
                    if (_buffer.Count < total)
                    {
                        break;//终止对本次接收到数据解析，保留缓冲区数据到下次数据接收
                    }
                    //CRC32一致性验证
                    byte[] data = _buffer.Slice(2 + 1 + 4, length).ToArray();
                    uint receiveCrc32 = _buffer.Slice(7 + length, 4).ToUInt32(0, Endianness.Big);
                    uint calcCrc32 = SecurityUtil.CRC32(data);
                    if (calcCrc32 != receiveCrc32)
                    {
                        Logger.LogError(string.Format(LangProvider.Instance.AnalysisFrameErrorMessage, string.Format(LangProvider.Instance.Crc32NotMatchMessage, $"Rx: {data.ToHex()} RxCRC32: {receiveCrc32.ToBytes(Endianness.Big).ToHex()} CaCRC32: {calcCrc32.ToBytes(Endianness.Big).ToHex()}")));
                        _buffer.RemoveRange(0, total);
                        continue;
                    }

                    ushort ending = _buffer.Slice(2 + 1 + 4 + length + 4, 2).ToUInt16(0, Endianness.Big);
                    if (ending != FrameEnding)
                    {
                        Logger.LogError(string.Format(LangProvider.Instance.AnalysisFrameErrorMessage, LangProvider.Instance.FrameEndingNotMatchMessage));
                        _buffer.RemoveRange(0, total);
                        continue;
                    }
                    ZoneFunctionCode code = (ZoneFunctionCode)_buffer[2];
                    _buffer.RemoveRange(0, total);
                    await AnalysisFrame(code, data, receiveCrc32.ToBytes(Endianness.Big));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format(LangProvider.Instance.AnalysisFrameErrorMessage, $"{ex.Message}"));
            }
            finally
            {
                _receiveLock.Release();
            }
        }
        private async Task AnalysisFrame(ZoneFunctionCode code, byte[] data, byte[] checkBytes)
        {
            if (Device.Instance.Mode == DeviceMode.Administrator && FrameFilterUtility.Filter(Logger, code))
            {
                Logger.LogDebug($"Rx: [ {code.GetString()} ](0x{Convert.ToUInt64(code):X2}) [ CRC32 {checkBytes.ToHex()} ] {data?.ToHex()}");
            }

            try
            {
                int index = 0;
                switch (code)
                {
                    case ZoneFunctionCode.Version:
                        {
                            AddBoard(data);
                            break;
                        }
                    case ZoneFunctionCode.ControlAndState:
                        {
                            ZoneState state = (ZoneState)data[index++];
                            LoadMode mode = (LoadMode)data[index++];

                            var burnInfo = BurnInfo;
                            if (burnInfo is not null)
                            {
                                burnInfo.LoadMode = mode;
                            }

                            byte[] temp = new byte[8];
                            if (state is not ZoneState.Error)
                            {
                                UpdateZoneState(state, null, temp);
                            }
                            else
                            {
                                ZoneErrorType error = (ZoneErrorType)data[index++];
                                Array.Copy(data, index, temp, 0, temp.Length);
                                UpdateZoneState(state, error, temp);
                            }

                            break;
                        }
                    case ZoneFunctionCode.BurnInBoardInsertState:
                        {
                            byte value = data[index];
                            Pluged = (value & 0x3) == 0x3;
                            Logger.LogInfo(Pluged ? LangProvider.Instance.PluggedMessage : LangProvider.Instance.UnpluggedMessage);
                            break;
                        }
                    case ZoneFunctionCode.DpsState:
                        {
                            byte mask = data[index++];
                            int count = data[index++];
                            for (int i = 0; i < count; i++)
                            {
                                int id = data[index++];
                                int slot = data[index++];
                                DpsType type = (DpsType)data[index++];

                                int inputVol = data.ToInt32(index, Endianness.Big);
                                index += 4;

                                int outputVol = data.ToInt32(index, Endianness.Big);
                                index += 4;

                                int current = data.ToInt32(index, Endianness.Big);
                                index += 4;

                                int temp = data.ToInt32(index, Endianness.Big);
                                index += 4;
                                DpsState state = (DpsState)data[index++];

                                if (type == DpsType.DPS)
                                {
                                    Dps dps = Dpss.FirstOrDefault(j => j.Id == id);
                                    dps?.Update(state, inputVol, outputVol, current, temp);
                                }
                                else if (type == DpsType.VIH)
                                {
                                    Vih vih = Vihs.FirstOrDefault(j => j.Id == id);
                                    if (vih is not null)
                                    {
                                        vih.OutputVoltage = outputVol;
                                        vih.State = state;
                                    }
                                }
                            }
                            break;
                        }
                    case ZoneFunctionCode.CurrentDpsPlan:
                        {
                            var burnInfo = BurnInfo;
                            int id = data[index++];
                            if (id != 0xFF && id < burnInfo.DpsPlans.Length)
                            {
                                var dpsPlan = burnInfo.DpsPlans[id];
                                if (burnInfo.CurrentDpsPlan != dpsPlan)
                                {
                                    burnInfo.CurrentDpsPlan = dpsPlan;

                                    foreach (Dps dps in Dpss)
                                    {
                                        dps.BurnDps = dpsPlan.Dpss.FirstOrDefault(t => t.Id == dps.Id);
                                    }
                                }
                            }
                            break;
                        }
                    case ZoneFunctionCode.BurnState:
                        {
                            var burnInfo = BurnInfo;

                            if (burnInfo is null ||
                                burnInfo.Ic is null)
                            {
                                return;
                            }
                            int total = data.ToInt32(index, Endianness.Big);
                            index += 4;
                            burnInfo.TotalSeconds = total;

                            int id = data[index++];

                            int changed = 0;

                            if (id < burnInfo.Steps.Length)
                            {
                                if (burnInfo.CurrentStep is null ||
                                    burnInfo.CurrentStep.Name != burnInfo.Steps[id].Name)
                                {
                                    burnInfo.CurrentStep = burnInfo.Steps[id];
                                    changed++;
                                }
                            }

                            burnInfo.RunInfo.Mode = (ExecuteMode)data[index++];

                            total = data.ToInt32(index, Endianness.Big);
                            index += 4;

                            int cost = data.ToInt32(index, Endianness.Big);
                            index += 4;
                            burnInfo.RunInfo.Progress.Update(total, cost);

                            burnInfo.LoopInfo.Enable = Convert.ToBoolean(data[index++]);

                            id = data[index++];
                            if (id < burnInfo.Steps.Length)
                            {
                                burnInfo.LoopInfo.Start = burnInfo.Steps[id];
                            }

                            id = data[index++];
                            if (id < burnInfo.Steps.Length)
                            {
                                burnInfo.LoopInfo.End = burnInfo.Steps[id];
                            }

                            burnInfo.LoopInfo.Mode = (ExecuteMode)data[index++];

                            total = data.ToInt32(index, Endianness.Big);
                            index += 4;

                            cost = data.ToInt32(index, Endianness.Big);
                            index += 4;
                            burnInfo.LoopInfo.Progress.Update(total, cost);

                            id = data[index++];
                            if (burnInfo.CurrentStep is not null &&
                                id < burnInfo.CurrentStep.Vectors.Length)
                            {
                                var vectorStep = burnInfo.CurrentStep.Vectors[id];
                                if (burnInfo.CurrentVector != vectorStep)
                                {
                                    changed++;
                                    burnInfo.CurrentVector = vectorStep;
                                    try
                                    {
                                        Register registers = RegisterParser.Parse(vectorStep.RegisterFile);
                                        foreach (Vih vih in Vihs)
                                        {
                                            vih.SetVoltage = registers.VIH.TryGetValue(vih.Id, out float value) ? (int)(value * 1000) : 0;
                                        }
                                    }
                                    catch
                                    {
                                        foreach (Vih vih in Vihs)
                                        {
                                            vih.SetVoltage = 0;
                                        }
                                    }
                                }
                            }
                            if (changed > 0)
                            {
                                Logger.LogInfo(string.Format(LangProvider.Instance.StepRunInfoMessage, burnInfo.CurrentStep.Name, burnInfo.CurrentVector.PatternFile.FileName()));
                            }

                            break;
                        }
                    case ZoneFunctionCode.CompareError:
                        {
                            var burnInfo = BurnInfo;
                            if (burnInfo is null)
                            {
                                return;
                            }

                            // 当前步次
                            int stepId = data[index++];
                            if (stepId >= burnInfo.Steps.Length)
                            {
                                return;
                            }
                            string step = burnInfo.Steps[stepId].Name;

                            // 当前子步
                            int vectorStepId = data[index++];
                            if (vectorStepId >= burnInfo.Steps[stepId].Vectors.Length)
                            {
                                return;
                            }
                            BurnVector vectorStep = burnInfo.Steps[stepId].Vectors[vectorStepId];
                            string pattern = vectorStep.PatternFile.FileName();
                            string register = vectorStep.RegisterFile.FileName();

                            // 错误数量
                            int count = data.ToInt16(index, Endianness.Big);
                            index += 2;

                            for (int i = 0; i < count; i++)
                            {
                                int cs = data[index++];  // 片选
                                int line = data.ToInt32(index, Endianness.Big);    // 向量所在行数
                                index += 4;

                                byte[] mask = new byte[32];
                                Array.Copy(data, index, mask, 0, mask.Length);
                                index += 32;

                                int[] pins = mask.MaskToIntegers();
                                foreach (int pin in pins)
                                {
                                    ulong id = CompareError.CreateCompareErrorId(stepId, vectorStepId, cs, line, pin);
                                    CompareError error = new CompareError
                                    {
                                        Id = id,
                                        Cs = cs,
                                        Dr = pin,
                                        Step = step,
                                        Pattern = pattern,
                                        Register = register,
                                        Line = line
                                    };

                                    _errorChannel.Writer.TryWrite((burnInfo, error));
                                }
                            }
                            break;
                        }
                    case ZoneFunctionCode.Trigger:
                        {
                            OutputType = (TriggerOutputType)data[index++];
                            OutputPin = OutputType == TriggerOutputType.Signal ? data[index++] : null;
                            break;
                        }
                    case ZoneFunctionCode.Register:
                        {
                            // 单个寄存器数据请求
                            byte id = data[index];
                            _ = SendRegisterFile(id);
                            break;
                        }
                    case ZoneFunctionCode.Pattern:
                        {
                            // 单个向量数据请求
                            byte id = data[index];
                            _ = SendPatternFile(id);
                            break;
                        }
                    case ZoneFunctionCode.Upgrade:
                        {
                            OnUpgradeProgress(data);
                            break;
                        }
                    case ZoneFunctionCode.UpgradeFinish:
                        {
                            OnUpgradeFinish();
                            break;
                        }
                    case ZoneFunctionCode.DeviceCheck:
                        {
                            DeviceCheckItem checkItem = DeviceCheckItem.FromBytes(data);
                            this.InvokeOnUI(() => DeviceCheckItems.Add(checkItem));
                            break;
                        }
                    case ZoneFunctionCode.DpsCheck:
                        {
                            int current = data.ToInt32(index, Endianness.Big) + 1;
                            index += 4;

                            int total = data.ToInt32(index, Endianness.Big);
                            index += 4;

                            int readTimes = data.ToInt32(index, Endianness.Big) + 1;
                            index += 4;

                            int setTimes = data.ToInt32(index, Endianness.Big);
                            index += 4;

                            int id = data[index++];

                            //int inputVol = data.ToInt32(index, Endianness.Big);   // 输入电压
                            index += 4;

                            int outputVol = data.ToInt32(index, Endianness.Big);  // 输出电压
                            index += 4;

                            int outputCur = data.ToInt32(index, Endianness.Big);  // 输出电流
                            index += 4;

                            int temperature = data.ToInt32(index, Endianness.Big);// 电源温度
                            index += 4;

                            DpsState state = (DpsState)data[index++]; // 电源工作状态

                            var dps = Dpss.FirstOrDefault(j => j.Id == id);

                            if (dps is not null)
                            {
                                if (state == DpsState.Runing)
                                {
                                    Logger.LogInfo(string.Format(LangProvider.Instance.DpsCheckResultMessage, current, total, dps.Name, (outputVol * 1F / 1000).ToString("F3").PadLeft(6, ' '), (outputCur * 1F / 1000).ToString("F3").PadLeft(6, ' '), readTimes, setTimes));
                                }
                                else if (state > 0 && (byte)state < 13)
                                {
                                    Logger.LogError(string.Format(LangProvider.Instance.DpsCheckResultMessage, current, total, dps.Name, (outputVol * 1F / 1000).ToString("F3").PadLeft(6, ' '), (outputCur * 1F / 1000).ToString("F3").PadLeft(6, ' '), readTimes, setTimes));
                                }
                            }
                            break;
                        }
                    case ZoneFunctionCode.ThermostatPID:
                        {
                            int count = data[index++];
                            for (int i = 0; i < count; i++)
                            {
                                byte id = data[index++];

                                int kp = data.ToInt32(index, Endianness.Big);
                                index += 4;

                                int ki = data.ToInt32(index, Endianness.Big);
                                index += 4;

                                int kd = data.ToInt32(index, Endianness.Big);
                                index += 4;

                                var thermostat = BurnInfo?.BurnInBoard.Thermostats.FirstOrDefault(t => t.Id == id);
                                if (thermostat != null)
                                {
                                    thermostat.Kp = kp;
                                    thermostat.Ki = ki;
                                    thermostat.Kd = kd;
                                }
                            }
                            break;
                        }
                    case ZoneFunctionCode.Thermostat:
                        {
                            int count = data[index++];
                            for (int i = 0; i < count; i++)
                            {
                                byte id = data[index++];
                                bool enable = Convert.ToBoolean(data[index++]);
                                int temperature = data.ToInt32(index, Endianness.Big);
                                index += 4;
                                var thermostat = BurnInfo?.BurnInBoard.Thermostats.FirstOrDefault(t => t.Id == id);
                                if (thermostat != null)
                                {
                                    thermostat.Update(enable, temperature);
                                }
                            }
                            break;
                        }
                }

            }
            catch (Exception e)
            {
                throw new Exception($"Rx: [ {code.GetString()} ](0x{Convert.ToUInt64(code):X2}) {data?.ToHex()} {e.Message}");
            }
        }
        private async Task SendAsync(ZoneFunctionCode code, params byte[] data)
        {
            if (!Connected)
            {
                return;
            }

            bool semaphoreHeld = false;
            try
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
                semaphoreHeld = true;

                int length = data?.Length ?? 0;
                byte[] frame = new byte[length + 13];

                int index = 0;
                // 帧头
                Array.Copy(FrameHeader.ToBytes(Endianness.Big), 0, frame, index, 2);
                index += 2;
                // 功能代码
                frame[index++] = (byte)code;
                // 数据长度
                Array.Copy(length.ToBytes(Endianness.Big), 0, frame, index, 4);
                index += 4;
                // 数据内容
                if (data is not null)
                {
                    Array.Copy(data, 0, frame, index, data.Length);
                    index += data.Length;
                }
                // CRC32校验
                uint check = 0u;
                if (data is not null)
                {
                    check = SecurityUtil.CRC32(data);
                }
                byte[] checkBytes = check.ToBytes(Endianness.Big);
                Array.Copy(checkBytes, 0, frame, index, 4);
                index += 4;
                // 帧尾
                Array.Copy(FrameEnding.ToBytes(Endianness.Big), 0, frame, index, 2);

                // 发送
                await _client.SendAsync(frame).ConfigureAwait(false);


                if (Device.Instance.Mode == DeviceMode.Administrator && FrameFilterUtility.Filter(Logger, code))
                {
                    Logger.LogDebug($"Tx: [ {code.GetString()} ](0x{Convert.ToUInt64(code):X2}) [ CRC32 {checkBytes.ToHex()} ] {data?.ToHex()}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Tx: [ {code.GetString()} ](0x{Convert.ToUInt64(code):X2}) Error: {ex.Message}");
            }
            finally
            {
                if (semaphoreHeld)
                {
                    _semaphore.Release();
                }
            }
        }
        private async Task SendAsync(ZoneFunctionCode code, Stream stream, Action<int> readCallback, params byte[] beforeStream)
        {
            if (!Connected)
            {
                return;
            }

            bool semaphoreHeld = false;
            try
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
                semaphoreHeld = true;

                // 计算数据长度，超过 4096 字节则分多次发送，否则直接调用 SendAsync 发送
                long beforeLength = beforeStream?.Length ?? 0;
                long streamLength = stream?.Length ?? 0;
                long length = beforeLength + streamLength;
                if (length > uint.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(length));
                }

                // 大包：帧头 + 数据流式发送 + 帧尾
                // 发送帧头
                byte[] header = BuildFrameHeader(code, (uint)length);
                await _client.SendAsync(header).ConfigureAwait(false);
                // 发送数据内容
                uint check = SecurityUtil.CRC32(Array.Empty<byte>()); // 初始化 CRC32
                if (beforeStream is { Length: > 0 })
                {
                    check = SecurityUtil.CRC32(check, beforeStream);
                    await _client.SendAsync(beforeStream).ConfigureAwait(false);
                }
                if (stream is not null)
                {
                    byte[] buffer = new byte[1024];
                    int read;
                    while ((read = await stream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
                    {
                        if (!Connected)
                        {
                            return;
                        }
                        if (read == buffer.Length)
                        {
                            check = SecurityUtil.CRC32(check, buffer);
                            await _client.SendAsync(buffer).ConfigureAwait(false);
                        }
                        else
                        {
                            byte[] temp = new byte[read];
                            Array.Copy(buffer, 0, temp, 0, read);
                            check = SecurityUtil.CRC32(check, temp);
                            await _client.SendAsync(temp).ConfigureAwait(false);
                        }
                        readCallback?.Invoke(read);
                    }
                }
                // 发送帧尾
                byte[] checkBytes = check.ToBytes(Endianness.Big);
                byte[] tail = BuildFrameTail(check);
                await _client.SendAsync(tail).ConfigureAwait(false);

                if (FrameFilterUtility.Filter(Logger, code))
                {
                    Logger.LogInfo($"Tx: [ {code.GetString()} ](0x{Convert.ToUInt64(code):X2}) [ CRC32 {checkBytes.ToHex()} ] {beforeStream?.ToHex()}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Tx: [ {code.GetString()} ](0x{Convert.ToUInt64(code):X2}) Error: {ex.Message}");
            }
            finally
            {
                if (semaphoreHeld)
                {
                    _semaphore.Release();
                }
            }
        }
        private static byte[] BuildFrameHeader(ZoneFunctionCode code, uint length)
        {
            byte[] header = new byte[7];
            int index = 0;
            //帧头
            Array.Copy(FrameHeader.ToBytes(Endianness.Big), 0, header, index, 2);
            index += 2;
            //功能码
            header[index++] = (byte)code;
            //数据长度
            Array.Copy(length.ToBytes(Endianness.Big), 0, header, index, 4);
            return header;
        }
        private static byte[] BuildFrameTail(uint check)
        {
            byte[] tail = new byte[6];
            int index = 0;
            Array.Copy(check.ToBytes(Endianness.Big), 0, tail, index, 4);
            index += 4;
            Array.Copy(FrameEnding.ToBytes(Endianness.Big), 0, tail, index, 2);
            return tail;
        }
        #endregion

        #region 资源释放
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            //循环任务的释放
            StopTicker();

            //释放老化信息中的老化日志
            BurnInfo?.Dispose();
            BurnInfo = null;

            //释放连接
            if (_client is not null)
            {
                _client.StateChanged -= OnStateChanged;
                _client.ReceivedData -= OnReceivedData;
                _client.Dispose();
                _client = null;
            }

            //释放区日志
            Logger?.Dispose();
            Logger = null;

            //释放信号和锁
            _semaphore.Dispose();
            _receiveLock.Dispose();

            _disposed = true;
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
