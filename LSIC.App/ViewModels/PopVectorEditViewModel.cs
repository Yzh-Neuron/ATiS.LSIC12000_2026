namespace LSIC.App.ViewModels
{
    public class PopVectorEditViewModel : ObservableObject
    {
        public Vector Vector { get; private set; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SelectPatternFileCommand { get; private set; }
        public ICommand SelectRegisterFileCommand { get; private set; }
        public PopVectorEditViewModel(Vector vector)
        {
            Vector = vector;
            ConfirmCommand = new RelayCommand(Confirm, CanConfirm);
            CancelCommand = new RelayCommand(Cancel);
            SelectPatternFileCommand = new RelayCommand(SelectPatternFile);
            SelectRegisterFileCommand = new RelayCommand(SelectRegisterFile);
        }
        private void SelectPatternFile()
        {
            string filter = $"{LangProvider.Instance.PatternFileText}|*.abm";
            string currentFile = Vector.PatternFile;

            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopVectorEditMessage.OpenFile, [filter, currentFile])).First();

            if (msgResult.Result is string filePath)
            {
                Vector.PatternFile = filePath;
            }
        }
        private void SelectRegisterFile()
        {
            string filter = $"{LangProvider.Instance.RegisterFileText}|*.reg";
            string currentFile = Vector.PatternFile;

            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopVectorEditMessage.OpenFile, [filter, currentFile])).First();

            if (msgResult.Result is string filePath)
            {
                Vector.RegisterFile = filePath;
            }
        }
        private bool CanConfirm()
        {
            return Vector.GetAllExistingErrorsRecursive().Count == 0;
        }
        private void Confirm()
        {
           MessageBus.Default.Send(MsgArgs.From(PopVectorEditMessage.Confirm, [Vector]));
        }
        private void Cancel()
        {
            MessageBus.Default.Send(MsgArgs.From(PopVectorEditMessage.Cancel, []));
        }
    }
}
