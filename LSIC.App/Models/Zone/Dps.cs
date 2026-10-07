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
            set => SetProperty(ref _burnDps, value);
        }
        public DpsState State { get; private set; } = DpsState.Idle;
        public int InputVoltage { get; private set; }
        public int OutputVoltage { get; private set; }
        public int OutputCurrent { get; private set; }
        public int OutputPower => OutputVoltage * OutputCurrent;
        public int Temperature { get; private set; }
        public Dps(DpsInfo dpsInfo)
        {
            Id = dpsInfo.Id;
            Name = dpsInfo.Name;
        }
        public void Update(DpsState state, int inputVoltage, int outputVoltage, int outputCurrent, int temperature)
        {
            State = state;
            InputVoltage = inputVoltage;
            OutputVoltage = outputVoltage;
            OutputCurrent = outputCurrent;
            Temperature = temperature;

            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(InputVoltage));
            OnPropertyChanged(nameof(OutputVoltage));
            OnPropertyChanged(nameof(OutputCurrent));
            OnPropertyChanged(nameof(OutputPower));
            OnPropertyChanged(nameof(Temperature));
        }
    }
}
