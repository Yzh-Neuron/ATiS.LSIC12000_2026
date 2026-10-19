namespace LSIC.App.ViewModels
{
    public class CtrBurnInBoardViewModel : ObservableObject
    {
        private Zone _zone;
        public Zone Zone
        {
            get => _zone;
            set => SetProperty(ref _zone, value);
        }
        private BurnInBoard _burnInBoard = null;
        public BurnInBoard BurnInBoard
        {
            get => _burnInBoard;
            set => SetProperty(ref _burnInBoard, value);
        }
        private BurnDut _selectedBurnDut = null;
        public BurnDut SelectedBurnDut
        {
            get => _selectedBurnDut;
            set => SetProperty(ref _selectedBurnDut, value);
        }
        public ICommand ThermostatMonitorCommand { get; private set; }
        public CtrBurnInBoardViewModel()
        {
            ThermostatMonitorCommand = new RelayCommand<BurnThermostat>(ThermostatMonitor);
        }
        private void ThermostatMonitor(BurnThermostat thermostat)
        {
            if (Zone != null && BurnInBoard?.Duts?.FirstOrDefault(d => d.BurnThermostat == thermostat) is BurnDut burnDut)
            {
                MessageBus.Default.Send(MsgArgs.From(WinMainMessage.ThermostatMonitor, [Zone, burnDut]));
            }
        }
    }
}
