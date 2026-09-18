namespace LSIC.App.ViewModels
{
    public class PopInputNameViewModel : ObservableValidator
    {
        private static readonly char[] IllegalChars = ['\\', '/', ':', '*', '?', '.', '\"', '<', '>', '|'];
        private readonly IEnumerable<string> _filters;
        private string _name;
        [ValidationMetadata(Method = nameof(ValidateInputName))]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, true);
        }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }
        public PopInputNameViewModel(IEnumerable<string> filters)
        {
            _filters = filters;
            ConfirmCommand = new RelayCommand(Confirm, CanConfirm);
            CancelCommand = new RelayCommand(Cancel);
        }
        private void Confirm()
        {
            MessageBus.Default.Send(MsgArgs.From(PopInputNameMessage.Confirm, [Name]));
        }
        private bool CanConfirm()
        {
            if (ValidateInputName(Name) == null)
            {
                return true;
            }
            return false;
        }
        private void Cancel()
        {
            MessageBus.Default.Send(MsgArgs.From(PopInputNameMessage.Cancel, []));
        }
        private ValidationResult ValidateInputName(string value)
        {
            if (value.IsNullOrEmptyOrWhiteSpace())
            {
                return new ValidationResult(LangProvider.Instance.NameNotNullMessage);
            }

            if (_filters.Contains(value))
            {
                return new ValidationResult(LangProvider.Instance.NameExistMessage);
            }

            int len = 36;
            if (value.Length > len)
            {
                return new ValidationResult(string.Format(LangProvider.Instance.NameNotExceedMessage, len));
            }

            if (value.Any(i => IllegalChars.Contains(i)))
            {
                return new ValidationResult(string.Format(LangProvider.Instance.NameIllegalMessage, len));
            }

            return ValidationResult.Success;
        }

    }
}