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
    }
}
