namespace LSIC.App.Views
{
    public partial class WinStartupView : IRecipient<MsgArgs<WinStartupMessage>>
    {
        public WinStartupViewModel ViewModel { get; }
        public WinStartupView()
        {
            InitializeComponent();
            ViewModel = new WinStartupViewModel();
            DataContext = ViewModel;
            MessageBus.Default.Register(this);
            Loaded += OnLoaded;
        }
        public MessageResult Receive(MsgArgs<WinStartupMessage> e)
        {
            if (e.Message is WinStartupMessage.Initialized)
            {
                WinMainView main = new WinMainView();
                Application.Current.MainWindow = main;
                main.Show();
                main.Activate();
                Close();
            }

            return MessageResult.From(null);
        }
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            try
            {
                await ViewModel.StartCheck();
            }
            catch
            {
                // ignore
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MessageBus.Default.UnregisterAll(this);

            if (Application.Current.MainWindow == null)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
