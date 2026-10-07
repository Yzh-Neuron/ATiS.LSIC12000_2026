namespace LSIC.App.Models
{
    public class Zone : ObservableObject, IDisposable
    {
        private TcpClient _client;
        public Zone(ZoneInfo info, Dps[] dpss)
        {
            Info = info ?? throw new ArgumentNullException(nameof(info));
            Dpss = dpss;
        }
        public ZoneInfo Info { get; }
        public int Id => Info.Id;
        public string Ip => Info.Ip;
        public ushort Port => Info.Port;
        public FlowLogger FlowLogger { get; private set; }
        private ZoneState _state = ZoneState.Offline;
        public ZoneState State
        {
            get => _state;
            private set => SetProperty(ref _state, value);
        }

        #region 关联设备

        /// <summary>
        /// dps电源
        /// </summary>
        public Dps[] Dpss { get; }

        #endregion

        #region 老化相关
        private BurnInfo _burnInfo;
        public BurnInfo BurnInfo
        {
            get => _burnInfo;
            set => SetProperty(ref _burnInfo, value);
        }

        #endregion

        #region 器件下发
        public async Task SendIcAsync()
        {
            BurnInfo info = BurnInfo;
            if (info == null)
            {
                throw new InvalidOperationException("尚未进入老化，无法下发配置");
            }

            LoadMode mode = LoadMode.Default;
            long lineCount = info.PatternIndexByPath.Keys.Sum(file =>
            {
                try
                {
                    return PatternFileHelper.GetCommandCount(file);
                }
                catch
                {
                    return long.MaxValue;
                }
            });
            if (lineCount > 16_000_000 || info.ForceStepLoad) // 16M 行 或者 IC 强制分步加载
            {
                mode = LoadMode.Step;
            }
        }

        #endregion

        #region 辅助方法
        private void UpdateZoneState(ZoneState state, ZoneErrorType? error = null, byte[] data = null)
        {
            if (state == ZoneState.DeviceCheckFail)
            {
                FlowLogger.LogErrorFormat(LangProvider.Instance.StateMessage, state.GetString());
            }
            else if (state == ZoneState.Error)
            {
                FlowLogger.LogErrorFormat(LangProvider.Instance.StateMessage, state.GetString());
            }
            else if (state == ZoneState.Offline)
            {
                FlowLogger.LogWarnFormat(LangProvider.Instance.StateMessage, state.GetString());
            }
            else
            {
                FlowLogger.LogInfoFormat(LangProvider.Instance.StateMessage, state.GetString());
            }

            if (state == ZoneState.Burning)
            {
                BurnInfo?.BurnInBoard?.Burning();
            }
            else if (state == ZoneState.Stop || state == ZoneState.Error)
            {
                BurnInfo?.BurnInBoard?.Finished();
            }

            if (state == ZoneState.Error && error.HasValue && data != null)
            {
                int index = 0;
                switch (error.Value)
                {
                    case ZoneErrorType.Dps:
                        {
                            DpsState dpsState = (DpsState)data[index++];
                            DpsType dpsType = (DpsType)data[index++];
                            int id = data[index++];
                            switch (dpsType)
                            {
                                case DpsType.DPS:
                                    {
                                        Dps dps = Dpss.FirstOrDefault(i => i.Id == id);
                                        if (dps != null)
                                        {
                                            string message = string.Format(LangProvider.Instance.ZoneDpsErrorLog, dps.Name, dpsState.GetString());
                                            FlowLogger.LogError(message);
                                            PopHelper.ShowAlarm(Id + LangProvider.Instance.ZoneText, message);
                                        }
                                    }
                                    break;
                                case DpsType.VIH:
                                    {
                                        string message = string.Format(LangProvider.Instance.ZoneVIHErrorLog, id, dpsState.GetString());
                                        FlowLogger.LogError(message);
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
                            string message = string.Format(LangProvider.Instance.ZonePatternErrorLog, patternErrorType.GetString(), stepId, vectorStepId);
                            FlowLogger.LogError(message);
                            PopHelper.ShowAlarm(Id + LangProvider.Instance.ZoneText, message);
                        }
                        break;
                    case ZoneErrorType.Device:
                        {
                            string message = string.Format(LangProvider.Instance.ZoneDeviceErrorLog, data.ToHex());
                            FlowLogger.LogError(message);
                            PopHelper.ShowAlarm(Id + LangProvider.Instance.ZoneText, message);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            State = state;
        }

        public void Startup()
        {

        }

        public void Dispose()
        {

        }

        #endregion
    }
}
