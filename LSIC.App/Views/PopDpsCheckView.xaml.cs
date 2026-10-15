namespace LSIC.App.Views
{
    public partial class PopDpsCheckView : IRecipient<MsgArgs<PopDpsCheckMessage>>
    {
        public PopDpsCheckViewModel ViewModel { get; }

        public PopDpsCheckView()
        {
            InitializeComponent();
            ViewModel = new PopDpsCheckViewModel();
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }

        public MessageResult Receive(MsgArgs<PopDpsCheckMessage> e)
        {
            switch (e.Message)
            {
                case PopDpsCheckMessage.OpenFile:
                    {
                        string filter = e.Args[0] as string;
                        string current = e.Args[1] as string;
                        string initialDirectory = Path.Exists(current) ? Path.GetDirectoryName(current)! : Path.GetFullPath(".\\Scripts\\Dps");
                        OpenFileDialog dialog = new OpenFileDialog
                        {
                            Title = LangProvider.Instance.SelectFileText,
                            Filter = filter,
                            Multiselect = false,
                            InitialDirectory = initialDirectory
                        };
                        if (dialog.ShowDialog() is true)
                        {
                            return MessageResult.From(dialog.FileName);
                        }
                        break;
                    }
                case PopDpsCheckMessage.Message:
                    {
                        string message = e.Args[0] as string;
                        MessageType type = (MessageType)e.Args[1];
                        return MessageResult.From(MessagePop.Show(message, type));
                    }
                case PopDpsCheckMessage.Growl:
                    {
                        Growl.Show(new GrowlInfo
                        {
                            Message = e.Args[0] as string,
                            Type = (MessageType)e.Args[1]
                        });
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
