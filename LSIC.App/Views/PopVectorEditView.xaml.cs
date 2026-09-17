namespace LSIC.App.Views
{
    public partial class PopVectorEditView : IRecipient<MsgArgs<PopVectorEditMessage>>
    {
        public PopVectorEditViewModel ViewModel { get; }
        public PopVectorEditView()
        {
            InitializeComponent();
            ViewModel = new PopVectorEditViewModel();
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }

        public MessageResult Receive(MsgArgs<PopVectorEditMessage> e)
        {
            throw new NotImplementedException();
        }

        private void OnChoosePatternFileButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnChooseRegisterFileButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }


}

namespace LSIC.App.ViewModels
{
    public class PopVectorEditViewModel : ObservableObject
    {

    }
}

namespace LSIC.App.ViewMessages
{
    public class PopVectorEditMessage
    {
    }
}
