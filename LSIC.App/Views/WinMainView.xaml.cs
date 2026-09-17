namespace LSIC.App.Views
{
    public partial class WinMainView : IRecipient<MsgArgs<WinMainMessage>>
    {
        public WinMainViewModel ViewModel { get; }
        public WinMainView()
        {
            InitializeComponent();
            ViewModel = new WinMainViewModel();
            DataContext = ViewModel;
            MessageBus.Default.Register(this);
        }
        public MessageResult Receive(MsgArgs<WinMainMessage> e)
        {
            switch (e.Message)
            {
                case WinMainMessage.OpenIcEditView:
                    PopTemplateView popTemplateView = new PopTemplateView() { Owner = this };
                    popTemplateView.ShowDialog();

                    break;
                case WinMainMessage.Start:
                    break;
                case WinMainMessage.Pause:
                    break;
                case WinMainMessage.Continue:
                    break;
                case WinMainMessage.Stop:
                    break;
                case WinMainMessage.CleareAlarm:
                    break;
                case WinMainMessage.PatternConvert:
                    break;
                case WinMainMessage.SetTriggerChanne:
                    break;
                case WinMainMessage.OvenControl:
                    break;
                case WinMainMessage.ThermostatMonitor:
                    break;
                case WinMainMessage.Message:
                    break;
                default:
                    break;
            }

            return MessageResult.Null();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MessageBus.Default.UnregisterAll(this);
            Application.Current.Shutdown();
        }
    }
}
