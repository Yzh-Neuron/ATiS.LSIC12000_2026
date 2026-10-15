namespace LSIC.App.ViewModels
{
    public class CtrHomeViewModel : ObservableObject
    {
        public ReadOnlyObservableCollection<Zone> Zones { get; }
        public ICommand ZoneListViewCommand { get; }
        private Zone _currentZone;
        public Zone CurrentZone
        {
            get => _currentZone;
            set => SetProperty(ref _currentZone, value);
        }

        public CtrHomeViewModel()
        {
            ZoneListViewCommand = new RelayCommand(ZoneListView);

            ObservableCollection<Zone> zones = new ObservableCollection<Zone>(Device.Instance?.Zones ?? Array.Empty<Zone>());
            Zones = new ReadOnlyObservableCollection<Zone>(zones);
            CurrentZone = Zones.FirstOrDefault();
        }

        private void ZoneListView()
        {
            MessageBus.Default.Send(MsgArgs.From(WinMainMessage.ZoneListView, []));
        }
    }
}
