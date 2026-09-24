namespace LSIC.App.Views
{
    public partial class PopRunStrategyView : IRecipient<MsgArgs<PopRunStrategyMessage>>
    {
        public PopRunStrategyViewModel ViewModel { get; }
        public PopRunStrategyView()
        {
            InitializeComponent();
            ViewModel = new PopRunStrategyViewModel();
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }

        public MessageResult Receive(MsgArgs<PopRunStrategyMessage> e)
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
    public class PopRunStrategyViewModel : ObservableObject
    {

    }
}

namespace LSIC.App.ViewMessages
{
    public enum PopRunStrategyMessage
    {
    }
}
