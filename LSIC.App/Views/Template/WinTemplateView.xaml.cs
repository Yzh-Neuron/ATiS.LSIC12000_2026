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
    public class WinTemplateMessage
    {
    }
}
