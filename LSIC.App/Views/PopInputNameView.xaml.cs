namespace LSIC.App.Views
{
    public partial class PopInputNameView : IRecipient<MsgArgs<PopInputNameMessage>>
    {
        public string NameResult { get; private set; }
        public PopInputNameViewModel ViewModel { get; }
        public PopInputNameView(string title, IEnumerable<string> filters = null)
        {
            InitializeComponent();
            ViewModel = new PopInputNameViewModel(filters);
            DataContext = ViewModel;
            Title = title;

            MessageBus.Default.Register(this);
        }
        public MessageResult Receive(MsgArgs<PopInputNameMessage> e)
        {
            switch (e.Message)
            {
                case PopInputNameMessage.Confirm:
                    {
                        NameResult = e.Args[0] as string;
                        Close();
                        break;
                    }
                case PopInputNameMessage.Cancel:
                    {
                        Close();
                        break;
                    }
            }
            return null;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MessageBus.Default.UnregisterAll(this);
        }
    }
}