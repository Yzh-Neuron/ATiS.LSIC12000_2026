namespace LSIC.App.Views
{
    public partial class WinTemplateView : IRecipient<MsgArgs<WinTemplateMessage>>
    {
        public WinTemplateViewModel ViewModel { get; }
        public WinTemplateView()
        {
            InitializeComponent();
            ViewModel = new WinTemplateViewModel();
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }

        public MessageResult Receive(MsgArgs<WinTemplateMessage> e)
        {
            throw new NotImplementedException();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MessageBus.Default.UnregisterAll(this);
        }
    }


}

namespace LSIC.App.ViewModels
{
    public class WinTemplateViewModel : ObservableObject
    {

    }
}

namespace LSIC.App.ViewMessages
{
    public enum WinTemplateMessage
    {
    }
}
