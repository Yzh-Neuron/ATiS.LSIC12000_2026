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
        }
        public MessageResult Receive(MsgArgs<WinStartupMessage> e)
        {
            if (e.Message is WinStartupMessage.Initialized)
            {
                WinMainView main = new WinMainView();
                Application.Current.MainWindow = main;
                main.Show();
                main.Activate();
                PopHelper.Init();
                PopHelper.SetOwner(main);
                Close();
            }

            return MessageResult.From(null);
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MessageBus.Default.UnregisterAll(this);

            if (Application.Current.MainWindow is not WinMainView)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
