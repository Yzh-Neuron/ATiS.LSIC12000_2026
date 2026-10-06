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
        private void OnZoneGroupChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not ListBox listBox)
            {
                return;
            }

            Zone[] newGroup = (e.NewValue as IEnumerable<Zone>)?.ToArray();
            if (newGroup == null || !newGroup.Any())
            {
                return;
            }

            listBox.ItemsSource = newGroup;
            listBox.SelectedValue = newGroup.First();
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
