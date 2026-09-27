namespace LSIC.App.Views
{
    public partial class PopVectorEditView : IRecipient<MsgArgs<PopVectorEditMessage>>
    {
        public Vector Vector { get; private set; }
        public PopVectorEditViewModel ViewModel { get; }
        public PopVectorEditView(Vector vector)
        {
            InitializeComponent();
            ViewModel = new PopVectorEditViewModel(vector);
            DataContext = ViewModel;
            Vector = vector;

            MessageBus.Default.Register(this);
        }

        public MessageResult Receive(MsgArgs<PopVectorEditMessage> e)
        {
            switch (e.Message)
            {
                case PopVectorEditMessage.Confirm:
                    {
                        Vector = e.Args[0] as Vector;
                        Close();
                        break;
                    }
                case PopVectorEditMessage.Cancel:
                    {
                        Vector = null;
                        Close();
                        break;
                    }
                case PopVectorEditMessage.ShowValidateError:
                    {
                        List<ValidationResult> errors = e.Args[0] as List<ValidationResult>;
                        MessagePop.Show(errors.ToMessageString(), MessageType.Error);
                        break;
                    }
                case PopVectorEditMessage.SelectFile:
                    {
                        string filter = e.Args[0] as string;
                        string current = e.Args[1] as string;
                        string initialDirectory = Path.Exists(current) ? Path.GetDirectoryName(current)! : Path.GetFullPath(".\\Samples");
                        OpenFileDialog dialog = new OpenFileDialog
                        {
                            Filter = filter!,
                            Multiselect = false,
                            InitialDirectory = initialDirectory
                        };
                        if (dialog.ShowDialog() is true)
                        {
                            return MessageResult.From(dialog.FileName);
                        }
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
            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
            SelectPatternFileCommand = new RelayCommand(SelectPatternFile);
            SelectRegisterFileCommand = new RelayCommand(SelectRegisterFile);
        }
        private void SelectPatternFile()
        {
            string filter = $"{LangProvider.Instance.PatternFileText}|*.abm";
            string currentFile = Vector.PatternFile;

            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopVectorEditMessage.SelectFile, [filter, currentFile])).First();

            if (msgResult.Result is string filePath)
            {
                Vector.PatternFile = filePath;
            }
        }
        private void SelectRegisterFile()
        {
            string filter = $"{LangProvider.Instance.RegisterFileText}|*.reg";
            string currentFile = Vector.PatternFile;

            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopVectorEditMessage.SelectFile, [filter, currentFile])).First();

            if (msgResult.Result is string filePath)
            {
                Vector.PatternFile = filePath;
            }
        }
        private void Confirm()
        {
            List<ValidationResult> errors = Vector.GetAllErrorsRecursive();
            if (errors.Count > 0)
            {
                MessageBus.Default.Send(MsgArgs.From(PopVectorEditMessage.ShowValidateError, [errors]));
            }
            else
            {
                MessageBus.Default.Send(MsgArgs.From(PopVectorEditMessage.Confirm, [Vector]));
            }

        }
        private void Cancel()
        {
            MessageBus.Default.Send(MsgArgs.From(PopVectorEditMessage.Cancel, []));
        }
    }
}

namespace LSIC.App.ViewMessages
{
    public enum PopVectorEditMessage
    {
        Confirm,
        Cancel,
        ShowValidateError,
        SelectFile,
    }
}
