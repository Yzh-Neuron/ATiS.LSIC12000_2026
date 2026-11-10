namespace LSIC.App.Models
{
    public class Zone : TcpHangKeClient<ZoneFunctionCode>
    {
        private Task _tickTask;
        private CancellationTokenSource _tickCts;
        private const int MaxErrorWritesPerTick = 128;
        private readonly PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(500));
        private readonly Channel<(BurnInfo Info, CompareError Error)> _errorChannel;
        public ObservableCollection<DeviceCheckItem> DeviceCheckItems { get; } = new();
        public ZoneInfo Info { get; }
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
        public ErrorDatabase ErrorDatabase { get; private set; }
        public Dps[] Dpss { get; }
        public Vih[] Vihs { get; }
        public Zone(ZoneInfo info, Dps[] dpss) : base(info.Id, info.Ip, info.Port)
        {
            Info = info ?? throw new ArgumentNullException(nameof(info));
            Dpss = dpss;
            Vihs = [new Vih(0), new Vih(1)];

            _errorChannel = Channel.CreateBounded<(BurnInfo, CompareError)>(new BoundedChannelOptions(4096) { SingleReader = true, FullMode = BoundedChannelFullMode.DropWrite });
            ErrorDatabase = new ErrorDatabase(this);
            Logger = new FlowLogger(new RollingFileLogger($@".\Log\Flow\Zone {Id}\{{date:yyyyMMdd}}.log", RollingInterval.Day, retentionPeriod: SystemLog.RetentionPeriod));
            FrameFilterUtility.Register<ZoneFunctionCode>(Logger, (code) => true);
        }
        public override void Startup()
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

            base.Startup();
            StartTicker();
            Logger.LogInfo(LangProvider.Instance.WorkerThreadStartedMessage);
        }
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                StopTicker();
                BurnInfo?.Dispose();
                BurnInfo = null;
            }

            base.Dispose(disposing);
            _disposed = true;
        }
        protected override Task OnConnectedAsync()
        {
            return QueryVersionAsync();
        }
        protected override Task OnDisconnectedAsync()
        {
            UpdateZoneState(ZoneState.Offline);
            Pluged = false;
            this.InvokeOnUI(DeviceCheckItems.Clear);
            return Task.CompletedTask;
        }
        protected override Task OnFrameReceivedAsync(byte functionCode, byte[] data, byte[] checkBytes)
        {
            return AnalysisFrame((ZoneFunctionCode)functionCode, data, checkBytes);
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
                    if (4L * (block + 4) + 4 != stream.Length)
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
                using MemoryStream stream = new MemoryStream();

                int length = info.Steps.Length;
                stream.WriteByte((byte)length);

                for (int i = 0; i < length; i++)
                {
                    BurnStepVector step = info.Steps[i];

                    // 基础信息 索引、运行模式、运行时间、电源方案等
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
                }

                // 发送步次数据
                await SendAsync(ZoneFunctionCode.StepSet, stream.ToArray()).ConfigureAwait(false);

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
        public async Task QueryVersionAsync()
        {
            ClearBoards();
            await SendAsync(ZoneFunctionCode.Version);
        }
        public Task SendUpgrade(byte[] payload)
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
        public async ValueTask SendPatternFileAsync(byte index)
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
                if (4L * (block + 4) + 4 != stream.Length)
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
                _ = SendAsync(ZoneFunctionCode.ControlAndState, 0x02);
            }
        }
        public async ValueTask SendRegisterFileAsync(byte index)
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
                _ = SendAsync(ZoneFunctionCode.ControlAndState, 0x02);
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
                            //已老化时间
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

                            //当前向量运行模式
                            burnInfo.RunInfo.Mode = (ExecuteMode)data[index++];

                            total = data.ToInt32(index, Endianness.Big);
                            index += 4;

                            int cost = data.ToInt32(index, Endianness.Big);
                            index += 4;

                            ////当前向量设定运行 时间/次数,已经消费 时间/次数
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
                            _ = SendRegisterFileAsync(id);
                            break;
                        }
                    case ZoneFunctionCode.Pattern:
                        {
                            // 单个向量数据请求
                            byte id = data[index];
                            _ = SendPatternFileAsync(id);
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

        #endregion

        #region 资源释放

        #endregion

        #region 固件升级
        public ObservableCollection<Board> Boards { get; } = new();
        private bool _upgradeSelected;
        public bool UpgradeSelected
        {
            get => _upgradeSelected;
            set => SetProperty(ref _upgradeSelected, value);
        }
        private UpgradeSession _session;
        public UpgradeSession Session
        {
            get => _session;
            set => SetProperty(ref _session, value);
        }
        public async Task SendFirmware(byte[] payload, int boardCount)
        {
            Session = new UpgradeSession(boardCount);
            await SendUpgrade(payload);
        }
        private void ClearBoards()
        {
            this.InvokeOnUI(Boards.Clear);
        }
        private void AddBoard(byte[] data)
        {
            var board = Board.FromBytes(data, BoardTypeCatalog.ByCode);
            this.InvokeOnUI(() => Boards.Add(board));
        }
        private void OnUpgradeProgress(byte[] data)
        {
            int index = 0;
            byte code = data[index++];              // 板卡类型
            byte id = data[index++];                // id
            byte progress = data[index++];          // 进度
            var state = (UpgradeState)data[index];  // 状态
            Session?.SetReport(code, id, progress, state);
        }
        private void OnUpgradeFinish()
        {
            Session?.Finish();
        }
        #endregion
    }
}
