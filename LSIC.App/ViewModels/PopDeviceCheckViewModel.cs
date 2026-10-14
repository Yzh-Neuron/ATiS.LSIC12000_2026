namespace LSIC.App.ViewModels
{
    public class PopDeviceCheckViewModel : ObservableObject
    {
        public Zone[] Zones { get; }
        public ICommand StartCommand { get; }
        private Zone _selectedZone;
        public Zone SelectedZone
        {
            get => _selectedZone;
            set => SetProperty(ref _selectedZone, value);
        }
        public PopDeviceCheckViewModel()
        {
            Zones = Device.Instance.Zones;
            StartCommand = new AsyncRelayCommand(StartAsync, CanStart);
            _selectedZone = Zones.FirstOrDefault();
        }
        private bool CanStart()
        {
            return SelectedZone != null && SelectedZone.State.CanCheck();
        }
        private async Task StartAsync()
        {
            await SelectedZone.StartDeviceCheckAsync();
        }
    }
}
