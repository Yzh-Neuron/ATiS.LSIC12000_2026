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
            ConfirmCommand = new RelayCommand(Confirm, CanConfirm);
            CancelCommand = new RelayCommand(Cancel);
        }
        private bool CanConfirm()
        {
            return ExecuteStrategy.GetAllExistingErrorsRecursive().Count == 0;
        }
        private void Confirm()
        {
            MessageBus.Default.Send(MsgArgs.From(PopExecuteStrategyMessage.Confirm, [ExecuteStrategy]));
        }
        private void Cancel()
        {
            MessageBus.Default.Send(MsgArgs.From(PopExecuteStrategyMessage.Cancel, []));
        }
    }
}
