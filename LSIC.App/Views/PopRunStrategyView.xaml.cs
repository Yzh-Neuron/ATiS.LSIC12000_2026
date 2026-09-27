namespace LSIC.App.Views
{
    public partial class PopRunStrategyView : IRecipient<MsgArgs<PopRunStrategyMessage>>
    {
        public bool? Result { get; private set; }
        public PopRunStrategyViewModel ViewModel { get; }
        public PopRunStrategyView(RunStrategy runStrategy)
        {
            InitializeComponent();
            ViewModel = new PopRunStrategyViewModel(runStrategy);
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }

        public MessageResult Receive(MsgArgs<PopRunStrategyMessage> e)
        {
            switch (e.Message)
            {
                case PopRunStrategyMessage.Confirm:
                    {
                        Result = true;
                        Close();
                        break;
                    }
                case PopRunStrategyMessage.Cancel:
                    {
                        Result = false;
                        Close();
                        break;
                    }
                case PopRunStrategyMessage.TerminateLoop:
                    {
                        Result = null;
                        Close();
                        break;
                    }
                case PopRunStrategyMessage.ShowValidateError:
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
    public class PopRunStrategyViewModel : ObservableObject
    {
        public RunStrategy RunStrategy { get; private set; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand TerminateLoopCommand { get; }

        public PopRunStrategyViewModel(RunStrategy runStrategy)
        {
            RunStrategy = runStrategy;
            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
            TerminateLoopCommand = new RelayCommand(TerminateLoop);
        }
        private void TerminateLoop()
        {
            MessageBus.Default.Send(MsgArgs.From(PopRunStrategyMessage.TerminateLoop, []));
        }
        private void Confirm()
        {
            List<ValidationResult> errors = RunStrategy.GetAllErrorsRecursive();
            if (errors.Count > 0)
            {
                MessageBus.Default.Send(MsgArgs.From(PopRunStrategyMessage.ShowValidateError, [errors]));
            }
            else
            {
                MessageBus.Default.Send(MsgArgs.From(PopRunStrategyMessage.Confirm, [RunStrategy]));
            }

        }
        private void Cancel()
        {
            MessageBus.Default.Send(MsgArgs.From(PopRunStrategyMessage.Cancel, []));
        }
    }
}

namespace LSIC.App.ViewMessages
{
    public enum PopRunStrategyMessage
    {
        Confirm,
        Cancel,
        TerminateLoop,
        ShowValidateError
    }
}
