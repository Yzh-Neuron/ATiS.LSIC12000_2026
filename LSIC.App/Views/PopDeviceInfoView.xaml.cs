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
                case PopDeviceInfoMessage.ExportCsv:
                    {
                        string fileName = e.Args[0] as string;
                        string filter = e.Args[1] as string;
                        var dialog = new SaveFileDialog
                        {
                            FileName = fileName,
                            Filter = filter
                        };
                        if (dialog.ShowDialog() != true)
                        {
                            return MessageResult.From(dialog.FileName);
                        }
                        break;
                    }
                case PopDeviceInfoMessage.Message:
                    {
                        string message = e.Args[0] as string;
                        MessageType type = (MessageType)e.Args[1];
                        return MessageResult.From(MessagePop.Show(message, type));
                    }
                default:
                    break;
            }
            return MessageResult.Null();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MessageBus.Default.UnregisterAll(this);
        }
    }
    public class PopDeviceInfoViewModelProxy : BindingProxy<PopDeviceInfoViewModel>
    {
        protected override Freezable CreateInstanceCore()
        {
            return new PopDeviceInfoViewModelProxy();
        }
    }
}
