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
                    {
                        WindowUtility.ShowOrActivate<PopIcEditView>();
                        break;
                    }
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
                    var rst = MessagePop.Show("文本", MessageType.Question);
                    Debug.WriteLine(rst);
                    break;
                case WinMainMessage.ThermostatMonitor:
                    break;
                case WinMainMessage.ShowValidateError:
                    {
                        List<ValidationResult> errors = e.Args[0] as List<ValidationResult>;
                        MessagePop.Show(errors.ToMessageString(), MessageType.Error);
                        break;
                    }
                case WinMainMessage.Message:
                    {
                        string message = e.Args[0] as string;
                        MessageType type = (MessageType)e.Args[1];
                        return MessageResult.From(MessagePop.Show(message, type));
                    }
                case WinMainMessage.ZoneListView:
                    {
                        var pop = new PopZoneListView
                        {
                            Owner = this
                        };

                        pop.ShowDialog();
                        break;
                    }

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
        private void OnAboutButtonClick(object sender, RoutedEventArgs e)
        {

            var pop = new PopAboutView
            {
                Owner = this
            };

            pop.ShowDialog();
        }

        private void OnDeviceInfoButtonClick(object sender, RoutedEventArgs e)
        {
            new PopDeviceInfoView { Owner = this }.ShowDialog();
        }

        private void OnUpgradeButtonClick(object sender, RoutedEventArgs e)
        {
            new PopUpgradeView { Owner = this }.ShowDialog();
        }

    }
}
