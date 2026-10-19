namespace LSIC.App.ViewModels
{
    public class PopThermostatViewModel : ObservableObject
    {
        public Guid Guid { get; } = Guid.NewGuid();
        public Zone Zone { get; }
        public BurnDut[] Duts { get; }
        private BurnDut _selectedDut;
        public BurnDut SelectedDut
        {
            get => _selectedDut;
            set
            {
                if (SetProperty(ref _selectedDut, value))
                {
                    SelectedThermostat = value?.BurnThermostat;
                }
            }
        }
        private BurnThermostat _selectedThermostat;
        public BurnThermostat SelectedThermostat
        {
            get => _selectedThermostat;
            set => SetProperty(ref _selectedThermostat, value);
        }
        public ICommand SetPIDCommand { get; }
        public PopThermostatViewModel(Zone zone, BurnDut selectedDut)
        {
            Zone = zone;
            Duts = zone.BurnInfo.BurnInBoard.Duts.Where(t => t.BurnThermostat != null).OrderBy(d => d.Id).ToArray();
            SelectedDut = Duts.FirstOrDefault(t => t == selectedDut);
            SetPIDCommand = new AsyncRelayCommand(SetPidAsync, CanSetPid);
        }
        private bool CanSetPid()
        {
            return Zone.Connected && SelectedThermostat != null;
        }
        private async Task SetPidAsync()
        {
            MessageBus.Default.Send(MsgArgs.From(PopThermostatMessage.SetPid, [SelectedThermostat]));
        }
    }
}