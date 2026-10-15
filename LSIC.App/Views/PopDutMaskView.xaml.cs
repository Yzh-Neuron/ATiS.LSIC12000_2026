namespace LSIC.App.Views
{
    public partial class PopDutMaskView : IRecipient<MsgArgs<PopDutMaskMessage>>
    {
        public byte[] DutMask { get; private set; }
        public PopDutMaskViewModel ViewModel { get; }

        public PopDutMaskView(BurnInBoard board)
        {
            InitializeComponent();
            ViewModel = new PopDutMaskViewModel(board);
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }

        public MessageResult Receive(MsgArgs<PopDutMaskMessage> e)
        {
            switch (e.Message)
            {
                case PopDutMaskMessage.Confirm:
                    {
                        DutMask = e.Args[0] as byte[];
                        Close();
                        break;
                    }
                case PopDutMaskMessage.Cancel:
                    {
                        DutMask = null;
                        Close();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return MessageResult.Null();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MessageBus.Default.UnregisterAll(this);
        }
    }
}