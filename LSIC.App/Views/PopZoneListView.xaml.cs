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
