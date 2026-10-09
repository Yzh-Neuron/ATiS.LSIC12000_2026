using System.Windows.Controls;

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

        private Zone _currentZone;
        public Zone CurrentZone
        {
            get => _currentZone;
            set => SetProperty(ref _currentZone, value);
        }

        public CtrHomeViewModel()
        {
            ObservableCollection<Zone> zones = new ObservableCollection<Zone>(Device.Instance?.Zones ?? Array.Empty<Zone>());
            Zones = new ReadOnlyObservableCollection<Zone>(zones);
            CurrentZone = Zones.FirstOrDefault();
        }
    }
}
