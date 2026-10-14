namespace LSIC.App.Views
{
    public partial class PopDeviceCheckView
    {
        public PopDeviceCheckViewModel ViewModel { get; }

        public PopDeviceCheckView()
        {
            InitializeComponent();
            ViewModel = new PopDeviceCheckViewModel();
            DataContext = ViewModel;
        }
    }
}