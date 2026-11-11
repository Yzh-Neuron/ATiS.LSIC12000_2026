namespace LSIC.App.Views
{
    public partial class PopControllerView
    {
        public PopControllerViewModel ViewModel { get; }
        public PopControllerView()
        {
            InitializeComponent();
            ViewModel = new PopControllerViewModel();
            DataContext = ViewModel;
        }
    }
}
