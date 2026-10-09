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
    }
}

namespace LSIC.App.ViewModels
{
    public class CtrBurnInfoViewModel : ObservableValidator
    {
        private Zone zone;
        public Zone Zone
        {
            get => zone;
            set => SetProperty(ref zone, value);
        }
        public ICommand SendIcCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand ClearBurnInfoCommand { get; private set; }
        public CtrBurnInfoViewModel()
        {
            SendIcCommand = new AsyncRelayCommand(SendIcAsync);
            StartCommand = new AsyncRelayCommand(StartAsync);
            StopCommand = new AsyncRelayCommand(StopAsync);
            ClearBurnInfoCommand = new RelayCommand(ClearBurnInfo);
        }
        private async Task SendIcAsync()
        {
            await Zone.SendIcAsync();
        }
        private async Task StartAsync()
        {
            var msgResult = MessageBus.Default.Send(MsgArgs.From(WinMainMessage.DutMask, [])).First();

            if (msgResult.Result is byte[] dutMask)
            {
                await Zone.StartBurnAsync(dutMask);
            }
        }
        private async Task StopAsync()
        {
            await Zone.StopBurnAsync();
        }
        private void ClearBurnInfo()
        {
            Zone.ResetStateToIdle();
        }

    }
}
