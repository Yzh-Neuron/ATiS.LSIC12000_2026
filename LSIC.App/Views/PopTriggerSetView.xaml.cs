namespace LSIC.App.Views
{
    public partial class PopTriggerSetView : IRecipient<MsgArgs<PopTriggerSetMessage>>
    {
        public string SuccessMessage { get; private set; }
        public PopTriggerSetViewModel ViewModel { get; }
        public PopTriggerSetView()
        {
            InitializeComponent();
            ViewModel = new PopTriggerSetViewModel();
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }

        public MessageResult Receive(MsgArgs<PopTriggerSetMessage> e)
        {
            switch (e.Message)
            {
                case PopTriggerSetMessage.Confirm:
                    {
                        SuccessMessage = e.Args[0] as string;
                        Close();
                        break;
                    }
                case PopTriggerSetMessage.Cancel:
                    {
                        SuccessMessage = null;
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