namespace LSIC.App.Views
{
    public partial class PopAboutView
    {
        public PopAboutViewModel ViewModel { get; }
        public PopAboutView()
        {
            InitializeComponent();
            ViewModel = new PopAboutViewModel();
            DataContext = ViewModel;
        }
    }
}
