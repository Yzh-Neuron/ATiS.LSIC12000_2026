namespace LSIC.App.Views
{
    public partial class PopZoneListView
    {
        public WinZoneListViewModel ViewModel { get; }
        public PopZoneListView()
        {
            InitializeComponent();
            ViewModel = new WinZoneListViewModel();
            DataContext = ViewModel;
        }
    }
}

namespace LSIC.App.ViewModels
{
    public class WinZoneListViewModel : ObservableObject
    {
        public Zone[] Zones { get; private set; }

        public WinZoneListViewModel()
        {
            Zones = Device.Instance.Zones.ToArray();
        }

    }
}
