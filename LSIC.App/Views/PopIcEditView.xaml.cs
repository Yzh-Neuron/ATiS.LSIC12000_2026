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
    public class PopIcEditViewModel : ObservableObject
    {

    }
}

namespace LSIC.App.ViewMessages
{
    public class PopIcEditMessage
    {
    }
}
