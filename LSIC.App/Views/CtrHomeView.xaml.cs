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
