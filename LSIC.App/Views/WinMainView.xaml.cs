namespace LSIC.App.Views
{
    /// <summary>
    /// WinMainView.xaml 的交互逻辑
    /// </summary>
    public partial class WinMainView 
    {
        public WinMainViewModel ViewModel { get; }
        public WinMainView()
        {
            InitializeComponent();
            ViewModel = new WinMainViewModel();
            DataContext = ViewModel;
        }
    }
}
