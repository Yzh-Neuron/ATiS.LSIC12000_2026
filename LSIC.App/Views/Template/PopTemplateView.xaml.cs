namespace LSIC.App.Views
{
    public partial class PopTemplateView : IRecipient<MsgArgs<PopTemplateMessage>>
    {
        public PopTemplateViewModel ViewModel { get; }
        public PopTemplateView()
        {
            InitializeComponent();
            ViewModel = new PopTemplateViewModel();
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }

        public MessageResult Receive(MsgArgs<PopTemplateMessage> e)
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
    public class PopTemplateViewModel : ObservableObject
    {

    }
}

namespace LSIC.App.ViewMessages
{
    public enum PopTemplateMessage
    {
    }
}
