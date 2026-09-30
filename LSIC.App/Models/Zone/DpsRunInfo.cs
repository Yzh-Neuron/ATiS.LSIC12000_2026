namespace LSIC.App.Models
{
    /// <summary>
    /// 电源类型（上报帧中区分）
    /// </summary>
    public enum DpsType : byte
    {
        Dps,
        Vih
    }

    /// <summary>
    /// 电源工作状态
    /// </summary>
    public enum DpsState : byte
    {
        Output = 0x00,
        Ovp = 0x01,
        Uvp = 0x02,
        Ocp = 0x04,
        Idle = 0x80
    }

    /// <summary>
    /// 直流电源运行信息
    /// </summary>
    public class DpsRunInfo : ObservableObject
    {
        public DpsRunInfo(DpsInfo info)
        {
            Info = info ?? throw new ArgumentNullException(nameof(info));
        }

        public DpsInfo Info { get; }

        private DpsSetting _setting;
        /// <summary>
        /// 当前方案中该电源的设置
        /// </summary>
        public DpsSetting Setting
        {
            get => _setting;
            set
            {
                if (SetProperty(ref _setting, value))
                {
                    OnPropertyChanged(nameof(Vol));
                    OnPropertyChanged(nameof(Uvp));
                    OnPropertyChanged(nameof(Ovp));
                    OnPropertyChanged(nameof(Ocp));
                }
            }
        }

        public int? Vol => Setting?.Vol;
        public int? Uvp => Setting?.Uvp;
        public int? Ovp => Setting?.Ovp;
        public int? Ocp => Setting?.Ocp;

        private DpsState _state = DpsState.Idle;
        public DpsState State
        {
            get => _state;
            private set => SetProperty(ref _state, value);
        }

        private int? _inputVoltage;
        public int? InputVoltage
        {
            get => _inputVoltage;
            private set => SetProperty(ref _inputVoltage, value);
        }

        private int? _outputVoltage;
        public int? OutputVoltage
        {
            get => _outputVoltage;
            private set => SetProperty(ref _outputVoltage, value);
        }

        private int? _outputCurrent;
        public int? OutputCurrent
        {
            get => _outputCurrent;
            private set => SetProperty(ref _outputCurrent, value);
        }

        public int? OutputPower => OutputVoltage * OutputCurrent;

        private int? _temperature;
        public int? Temperature
        {
            get => _temperature;
            private set => SetProperty(ref _temperature, value);
        }

        public void Update(DpsState state, int inputVoltage, int outputVoltage, int outputCurrent, int temperature)
        {
            State = state;
            InputVoltage = inputVoltage;
            OutputVoltage = outputVoltage;
            OutputCurrent = outputCurrent;
            Temperature = temperature;
            OnPropertyChanged(nameof(OutputPower));
        }

        public void Reset()
        {
            State = DpsState.Idle;
            InputVoltage = null;
            OutputVoltage = null;
            OutputCurrent = null;
            Temperature = null;
            OnPropertyChanged(nameof(OutputPower));
        }
    }
}
