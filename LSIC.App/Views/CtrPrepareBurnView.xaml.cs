namespace LSIC.App.Views
{
    public partial class CtrPrepareBurnView
    {
        public CtrPrepareBurnViewModel ViewModel { get; }
        public CtrPrepareBurnView()
        {
            InitializeComponent();
            ViewModel = new CtrPrepareBurnViewModel();
            DataContext = ViewModel;
        }
        public Zone Zone
        {
            get => (Zone)GetValue(ZoneProperty);
            set => SetValue(ZoneProperty, value);
        }
        public static readonly DependencyProperty ZoneProperty = DependencyProperty.Register(nameof(Zone), typeof(Zone), typeof(CtrPrepareBurnView), new PropertyMetadata(null, OnZoneChanged));
        private static void OnZoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CtrPrepareBurnView)?.UpdateZone(e.NewValue as Zone);
        }
        private void UpdateZone(Zone zone)
        {
            ViewModel.Zone = zone;
        }
    }
}
