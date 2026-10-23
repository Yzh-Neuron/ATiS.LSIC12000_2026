namespace LSIC.App.Models
{
    public class Dps : ObservableObject
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        private BurnDps _burnDps;
        public BurnDps BurnDps
        {
            get => _burnDps;
            set
            {
                if (SetProperty(ref _burnDps, value))
                {
                    OnPropertyChanged(nameof(Vol));
                    OnPropertyChanged(nameof(Uvp));
                    OnPropertyChanged(nameof(Ovp));
                    OnPropertyChanged(nameof(Ocp));
                }
            }
        }
        public int? Vol => BurnDps?.Vol;
        public int? Uvp => BurnDps?.Uvp;
        public int? Ovp => BurnDps?.Ovp;
        public int? Ocp => BurnDps?.Ocp;
        private DpsState _state = DpsState.Idle;
        public DpsState State
        {
            get => _state;
            private set => SetProperty(ref _state, value);
        }
        private int _temperature;
        public int Temperature
        {
            get => _temperature;
            private set => SetProperty(ref _temperature, value);
        }
        private int _inputVoltage;
        public int InputVoltage
        {
            get => _inputVoltage;
            private set => SetProperty(ref _inputVoltage, value);
        }
        private int _outputVoltage;
        public int OutputVoltage
        {
            get => _outputVoltage;
            private set => SetProperty(ref _outputVoltage, value);
        }
        private int _outputCurrent;
        public int OutputCurrent
        {
            get => _outputCurrent;
            private set => SetProperty(ref _outputCurrent, value);
        }
        private int _outputPower;
        public int OutputPower
        {
            get => _outputPower;
            private set => SetProperty(ref _outputPower, value);
        }
        public Dps(DpsInfo dpsInfo)
        {
            Id = dpsInfo.Id;
            Name = dpsInfo.Name;
        }
        public void Update(DpsState state, int inputVoltage, int outputVoltage, int outputCurrent, int temperature)
        {
            if (outputVoltage != _outputVoltage || outputCurrent != _outputCurrent)
            {
                OutputPower = outputVoltage * outputCurrent;
            }

            State = state;
            InputVoltage = inputVoltage;
            OutputVoltage = outputVoltage;
            OutputCurrent = outputCurrent;
            Temperature = temperature;
        }
    }
}
