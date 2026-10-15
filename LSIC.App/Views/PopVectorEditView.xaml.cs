namespace LSIC.App.Views
{
    public partial class PopVectorEditView : IRecipient<MsgArgs<PopVectorEditMessage>>
    {
        public Vector Vector { get; private set; }
        public PopVectorEditViewModel ViewModel { get; }
        public PopVectorEditView(Vector vector)
        {
            InitializeComponent();
            ViewModel = new PopVectorEditViewModel(vector);
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }

        public MessageResult Receive(MsgArgs<PopVectorEditMessage> e)
        {
            switch (e.Message)
            {
                case PopVectorEditMessage.Confirm:
                    {
                        Vector = e.Args[0] as Vector;
                        Close();
                        break;
                    }
                case PopVectorEditMessage.Cancel:
                    {
                        Vector = null;
                        Close();
                        break;
                    }
                case PopVectorEditMessage.OpenFile:
                    {
                        string filter = e.Args[0] as string;
                        string current = e.Args[1] as string;
                        string initialDirectory = Path.Exists(current) ? Path.GetDirectoryName(current)! : Path.GetFullPath(".\\Samples");
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
