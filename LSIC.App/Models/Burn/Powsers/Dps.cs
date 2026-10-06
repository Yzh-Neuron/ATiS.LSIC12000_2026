namespace LSIC.App.Models
{
    public class Dps : ObservableObject
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Vol { get; private set; }
        public int Uvp { get; private set; }
        public int Ovp { get; private set; }
        public int Ocp { get; private set; }
        public int Raise { get; private set; }
        public int Delay { get; private set; }
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
    }
}
