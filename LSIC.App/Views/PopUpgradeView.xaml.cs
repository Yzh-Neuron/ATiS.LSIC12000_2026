namespace LSIC.App.Views
{
    public partial class PopUpgradeView : IRecipient<MsgArgs<PopUpgradeMessage>>
    {
        public PopUpgradeViewModel ViewModel { get; }
        public PopUpgradeView()
        {
            InitializeComponent();
            ViewModel = new PopUpgradeViewModel();
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }
        public MessageResult Receive(MsgArgs<PopUpgradeMessage> e)
        {
            switch (e.Message)
            {
                case PopUpgradeMessage.SelectFile:
                    {
                        OpenFileDialog dialog = new OpenFileDialog
                        {
                            Filter = $"{LangProvider.Instance.FirmwareText}|*.bin|*|*.*",
                        };
                        if (dialog.ShowDialog() is true)
                        {
                            return MessageResult.From(dialog.FileName);
                        }
                        break;
                    }
                case PopUpgradeMessage.Message:
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
            ViewModel?.Dispose();
        }
    }

    public class PopUpgradeViewModelProxy : BindingProxy<PopUpgradeViewModel>
    {
        protected override Freezable CreateInstanceCore()
        {
            return new PopUpgradeViewModelProxy();
        }
    }
}
