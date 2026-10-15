namespace LSIC.App.Views
{
    public partial class PopDeviceInfoView : IRecipient<MsgArgs<PopDeviceInfoMessage>>
    {
        public PopDeviceInfoViewModel ViewModel { get; }
        public PopDeviceInfoView()
        {
            InitializeComponent();
            ViewModel = new PopDeviceInfoViewModel();
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }
        public MessageResult Receive(MsgArgs<PopDeviceInfoMessage> e)
        {
            switch (e.Message)
            {
                case PopDeviceInfoMessage.Export:
                    {
                        string filter = e.Args[0] as string;
                        string current = e.Args[1] as string;
                        SaveFileDialog dialog = new SaveFileDialog
                        {
                            Title = LangProvider.Instance.ExportText,
                            Filter = filter,
                            FileName = current,
                            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
                        };
                        if (dialog.ShowDialog() is true)
                        {
                            return MessageResult.From(dialog.FileName);
                        }
                        break;
                    }
                case PopDeviceInfoMessage.Growl:
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
