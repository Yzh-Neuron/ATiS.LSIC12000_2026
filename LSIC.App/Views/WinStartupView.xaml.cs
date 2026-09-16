namespace LSIC.App.Views
{
    public partial class WinStartupView : IRecipient<MsgArgs<WinStartupMessage>>
    {
        public WinStartupView()
        {
            InitializeComponent();
        }

        public MessageResult Receive(MsgArgs<WinStartupMessage> message)
        {
            throw new NotImplementedException();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (Application.Current.MainWindow == null)
            {
                Application.Current.Shutdown();
            }
        }

    }
}
