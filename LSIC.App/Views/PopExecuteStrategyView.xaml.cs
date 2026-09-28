namespace LSIC.App.Views
{
    public partial class PopExecuteStrategyView : IRecipient<MsgArgs<PopExecuteStrategyMessage>>
    {
        public ExecuteStrategy ExecuteStrategy { get; private set; }
        public PopExecuteStrategyViewModel ViewModel { get; }

        public PopExecuteStrategyView(ExecuteStrategy executeStrategy, bool isLoop = false)
        {
            InitializeComponent();
            ViewModel = new PopExecuteStrategyViewModel(executeStrategy);
            DataContext = ViewModel;

            MessageBus.Default.Register(this);

            if (isLoop)
            {
                Title = LangProvider.Instance.LoopStrategyText;
                modeText.Text = LangProvider.Instance.LoopModeText;
                timeText.Text = LangProvider.Instance.LoopTimeText;
                timesText.Text = LangProvider.Instance.LoopTimesText;
            }
            else
            {
                Title = LangProvider.Instance.RunStrategyText;
                modeText.Text = LangProvider.Instance.RunModeText;
                timeText.Text = LangProvider.Instance.RunTimeText;
                timesText.Text = LangProvider.Instance.RunTimesText;
            }
        }
        public MessageResult Receive(MsgArgs<PopExecuteStrategyMessage> e)
        {
            switch (e.Message)
            {
                case PopExecuteStrategyMessage.Confirm:
                    {
                        ExecuteStrategy = e.Args[0] as ExecuteStrategy;
                        Close();
                        break;
                    }
                case PopExecuteStrategyMessage.Cancel:
                    {
                        ExecuteStrategy = null;
                        Close();
                        break;
                    }
                case PopExecuteStrategyMessage.ShowValidateError:
                    {
                        List<ValidationResult> errors = e.Args[0] as List<ValidationResult>;
                        MessagePop.Show(errors.ToMessageString(), MessageType.Error);
                        break;
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
}

namespace LSIC.App.ViewModels
{
    public class PopExecuteStrategyViewModel : ObservableObject
    {
        public ExecuteStrategy ExecuteStrategy { get; private set; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }
        public PopExecuteStrategyViewModel(ExecuteStrategy executeStrategy)
        {
            ExecuteStrategy = executeStrategy;
            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
        }
        private void Confirm()
        {
            List<ValidationResult> errors = ExecuteStrategy.GetAllErrorsRecursive();
            if (errors.Count > 0)
            {
                MessageBus.Default.Send(MsgArgs.From(PopExecuteStrategyMessage.ShowValidateError, [errors]));
            }
            else
            {
                MessageBus.Default.Send(MsgArgs.From(PopExecuteStrategyMessage.Confirm, [ExecuteStrategy]));
            }

        }
        private void Cancel()
        {
            MessageBus.Default.Send(MsgArgs.From(PopExecuteStrategyMessage.Cancel, []));
        }
    }
}

namespace LSIC.App.ViewMessages
{
    public enum PopExecuteStrategyMessage
    {
        Confirm,
        Cancel,
        ShowValidateError
    }
}
