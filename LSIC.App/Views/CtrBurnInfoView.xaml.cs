using ATiS.Orm;
using System.Windows.Controls;

namespace LSIC.App.Views
{
    public partial class CtrBurnInfoView
    {
        public CtrBurnInfoViewModel ViewModel { get; }
        public CtrBurnInfoView()
        {
            InitializeComponent();
            ViewModel = new CtrBurnInfoViewModel();
            DataContext = ViewModel;
        }
        public Zone Zone
        {
            get => (Zone)GetValue(ZoneProperty);
            set => SetValue(ZoneProperty, value);
        }
        public static readonly DependencyProperty ZoneProperty = DependencyProperty.Register(nameof(Zone), typeof(Zone), typeof(CtrBurnInfoView), new PropertyMetadata(null, OnZoneChanged));
        private static void OnZoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CtrBurnInfoView)?.UpdateZone(e.NewValue as Zone);
        }
        private void UpdateZone(Zone zone)
        {
            ViewModel.Zone = zone;
        }

        private void OnViewRegisterFileButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Tag is string path && File.Exists(path))
            {
                PopTextBrowseView window = new PopTextBrowseView(path.FileName(), path, Encoding.UTF8)
                {
                    Owner = WindowExtension.GetActiveWindow()
                };
                window.ShowDialog();
            }
        }
    }
}
