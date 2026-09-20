namespace LSIC.App.Views
{
    public partial class PopIcEditView : IRecipient<MsgArgs<PopIcEditMessage>>
    {
        public PopIcEditViewModel ViewModel { get; }
        public PopIcEditView()
        {
            InitializeComponent();
            ViewModel = new PopIcEditViewModel();
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }

        public MessageResult Receive(MsgArgs<PopIcEditMessage> e)
        {
            switch (e.Message)
            {
                case PopIcEditMessage.InputName:
                    {
                        string title = e.Args[0] as string;
                        IEnumerable<string> filters = e.Args[1] as IEnumerable<string>;
                        PopInputNameView pop = new PopInputNameView(title, filters) { Owner = this };
                        pop.ShowDialog();
                        return MessageResult.From(pop.NameResult);
                    }
                case PopIcEditMessage.Message:
                    {
                        string message = e.Args[0] as string;
                        MessageType type = (MessageType)e.Args[1];
                        return MessageResult.From(MessagePop.Show(message, type));
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
