namespace LSIC.App.Views
{
    public partial class CtrHomeView
    {
        public CtrHomeViewModel ViewModel { get; }
        public CtrHomeView()
        {
            InitializeComponent();
            ViewModel = new CtrHomeViewModel();
            DataContext = ViewModel;
        }

        private void OnZoneListViewButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }

    public class CtrHomeViewModelProxy : BindingProxy<CtrHomeViewModel>
    {
        protected override Freezable CreateInstanceCore()
        {
            return new CtrHomeViewModelProxy();
        }
    }
}

namespace LSIC.App.ViewModels
{
    public class CtrHomeViewModel : ObservableObject
    {
        public ReadOnlyObservableCollection<Zone> Zones { get; private set; }

        public ICommand ZoneListViewCommand { get; private set; }
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
