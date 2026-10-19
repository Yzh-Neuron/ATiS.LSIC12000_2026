namespace LSIC.App.Views
{
    public partial class CtrBurnInBoardView
    {
        public CtrBurnInBoardViewModel ViewModel { get; }
        public CtrBurnInBoardView()
        {
            InitializeComponent();
            ViewModel = new CtrBurnInBoardViewModel();
            DataContext = ViewModel;
        }

        public BurnInBoard BurnInBoard
        {
            get => (BurnInBoard)GetValue(BurnInBoardProperty);
            set => SetValue(BurnInBoardProperty, value);
        }
        public static readonly DependencyProperty BurnInBoardProperty = DependencyProperty.Register(nameof(BurnInBoard), typeof(BurnInBoard), typeof(CtrBurnInBoardView), new PropertyMetadata(null, OnBurnInBoardChanged));
        private static void OnBurnInBoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CtrBurnInBoardView)?.UpdateBurnInfo(e.NewValue as BurnInBoard);
        }
        private void UpdateBurnInfo(BurnInBoard burnInBoard)
        {
            ViewModel.BurnInBoard = burnInBoard;
        }

        public Zone Zone
        {
            get => (Zone)GetValue(ZoneProperty);
            set => SetValue(ZoneProperty, value);
        }
        public static readonly DependencyProperty ZoneProperty = DependencyProperty.Register(nameof(Zone), typeof(Zone), typeof(CtrBurnInBoardView), new PropertyMetadata(null, OnZoneChanged));
        private static void OnZoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CtrBurnInBoardView)?.UpdateZone(e.NewValue as Zone);
        }
        private void UpdateZone(Zone zone)
        {
            ViewModel.Zone = zone;
        }
    }
    public class CtrBurnInBoardViewModelProxy : BindingProxy<CtrBurnInBoardViewModel>
    {
        protected override Freezable CreateInstanceCore()
        {
            return new CtrBurnInBoardViewModelProxy();
        }
    }
}
