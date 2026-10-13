namespace LSIC.App.Views
{
    public partial class PopDeviceInfoView
    {
        public PopDeviceInfoViewModel ViewModel { get; }

        public PopDeviceInfoView()
        {
            InitializeComponent();
            ViewModel = new PopDeviceInfoViewModel();
            DataContext = ViewModel;
        }
    }
    public class PopDeviceInfoViewModelProxy : BindingProxy<PopDeviceInfoViewModel>
    {
        protected override Freezable CreateInstanceCore()
        {
            return new PopDeviceInfoViewModelProxy();
        }
    }
}
