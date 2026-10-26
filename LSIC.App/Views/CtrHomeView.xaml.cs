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
        public void RefreshAllPrepareBurnViews()
        {
            foreach (var v in this.GetChildren<CtrPrepareBurnView>())
            {
                v.ViewModel.Refresh();
            }
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
