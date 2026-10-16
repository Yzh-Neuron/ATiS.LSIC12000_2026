namespace LSIC.App.ViewModels
{
    public class PopPermissionVerifyViewModel : ObservableValidator
    {
        private bool _InvalidPassword;
        [ValidationMetadata(Method = nameof(ValidateInvalidPassword))]
        public bool InvalidPassword
        {
            get => _InvalidPassword;
            set => SetProperty(ref _InvalidPassword, value, true);
        }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }
        public PopPermissionVerifyViewModel()
        {
            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
            ValidateAllProperties();
        }
        private void Confirm()
        {
            MessageBus.Default.Send(MsgArgs.From(PopPermissionVerifyMessage.Confirm, []));
        }
        private void Cancel()
        {
            MessageBus.Default.Send(MsgArgs.From(PopPermissionVerifyMessage.Cancel, []));
        }

        #region validate methods
        private ValidationResult ValidateInvalidPassword(bool value)
        {
            if (value)
            {
                return new ValidationResult(LangProvider.Instance.InvalidPasswordMessage);
            }

            return ValidationResult.Success;
        }
        #endregion
    }
}
