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
                case WinMainMessage.Growl:
                    {
                        Growl.Show(new GrowlInfo
                        {
                            Message = e.Args[0] as string,
                            Type = (MessageType)e.Args[1]
                        });
                        break;
                    }
                case WinMainMessage.ZoneListView:
                    {
                        new PopZoneListView { Owner = this }.ShowDialog();
                        break;
                    }
                case WinMainMessage.DutMask:
                    {
                        new PopZoneListView { Owner = this }.ShowDialog();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return MessageResult.Null();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            MessageBus.Default.UnregisterAll(this);
            MessageBus.Default.UnregisterAll(LogBrowse);
            Application.Current.Shutdown();
        }
        private void OnAboutButtonClick(object sender, RoutedEventArgs e)
        {
            new PopAboutView { Owner = this }.ShowDialog();
        }
        private void OnDeviceInfoButtonClick(object sender, RoutedEventArgs e)
        {
            new PopDeviceInfoView { Owner = this }.ShowDialog();
        }
        private void OnUpgradeButtonClick(object sender, RoutedEventArgs e)
        {
            new PopUpgradeView { Owner = this }.ShowDialog();
        }
        private void OnDeviceCheckButtonClick(object sender, RoutedEventArgs e)
        {
            new PopDeviceCheckView { Owner = this }.ShowDialog();
        }
        private void OnDpsCheckButtonClick(object sender, RoutedEventArgs e)
        {
            new PopDpsCheckView { Owner = this }.ShowDialog();
        }
        private void OnOpenIcEditButtonClick(object sender, RoutedEventArgs e)
        {
            WindowUtility.ShowOrActivate<PopIcEditView>();
        }
        private void OnPatternConvertButtonClick(object sender, RoutedEventArgs e)
        {

        }
        private void OnSetTriggerChannelButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
