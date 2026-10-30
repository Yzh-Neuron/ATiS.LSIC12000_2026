namespace LSIC.App.Models
{
    public class Vih : ObservableObject
    {
        public Vih(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }

        private int _setVoltage;
        public int SetVoltage
        {
            get => _setVoltage;
            set => SetProperty(ref _setVoltage, value);
        }

        private int _outputVoltage;
        public int OutputVoltage
        {
            get => _outputVoltage;
            set => SetProperty(ref _outputVoltage, value);
        }

        private DpsState _state = DpsState.Off;
        public DpsState State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }
    }
}
