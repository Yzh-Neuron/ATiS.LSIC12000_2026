namespace LSIC.App.Models
{
    public class Dut : ObservableObject
    {
        public int Id { get; }
        public int Row { get; }
        public int Column { get; }
        public int ChipSelect { get; }
        public int[] Drs { get; }
        public int[] Dpss { get; }
        public Thermostat Thermostat { get; internal set; }
        public Dut(DutSetting setting, Thermostat thermostat = null)
        {
            Id = setting.Id;
            Row = setting.Row; 
            Column = setting.Column;
            ChipSelect = setting.ChipSelect; 
            Drs = setting.Drs;
            Dpss = setting.Dpss; 
            Thermostat = thermostat;
        }
        public DutState State
        {
            get
            {
                return Masked ? DutState.Ignore :
                       Error ? DutState.Fail :
                       Finished ? DutState.Pass :
                       Burning ? DutState.Test : DutState.Idle;
            }
        }
        private bool _masked;
        public bool Masked
        {
            get => _masked;
            set => SetFlag(ref _masked, value);
        }
        private bool _burning;
        public bool Burning
        {
            get => _burning;
            set => SetFlag(ref _burning, value);
        }
        private bool _finished;
        public bool Finished
        {
            get => _finished;
            set => SetFlag(ref _finished, value);
        }
        private bool _error;
        public bool Error
        {
            get => _error;
            set => SetFlag(ref _error, value);
        }
        private void SetFlag(ref bool field, bool value)
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(State));
            }
        }
    }
}