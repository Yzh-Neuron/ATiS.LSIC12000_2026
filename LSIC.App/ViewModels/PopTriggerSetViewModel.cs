namespace LSIC.App.ViewModels
{
    public class PopTriggerSetViewModel : ObservableValidator
    {
        public Zone[] Zones { get; private set; }
        private Zone selectedZone;
        public Zone SelectedZone
        {
            get => selectedZone;
            set => SetProperty(ref selectedZone, value);
        }
        private TriggerOutputType selectedOutputType;
        public TriggerOutputType SelectedOutputType
        {
            get => selectedOutputType;
            set => SetProperty(ref selectedOutputType, value);
        }
        public int dr;
        [ValidationMetadata(Method = nameof(ValidateDr))]
        public int Dr
        {
            get => dr;
            set => SetProperty(ref dr, value, true);
        }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }
        public PopTriggerSetViewModel()
        {
            Zones = Device.Instance.Zones.ToArray();
            SelectedOutputType = TriggerOutputType.Signal;
            SelectedZone = Zones.FirstOrDefault();
            SelectedOutputType = SelectedZone?.OutputType ?? TriggerOutputType.Close;
            ConfirmCommand = new AsyncRelayCommand(ConfirmAsync, CanConfirm);
            CancelCommand = new RelayCommand(Cancel);
            ValidateAllProperties();
        }
        private bool CanConfirm()
        {
            return SelectedZone != null && Dr >= 0 && Dr <= 255;
        }
        private async Task ConfirmAsync()
        {

            await SelectedZone.TriggerAsync(SelectedOutputType, Dr);
            MessageBus.Default.Send(MsgArgs.From(PopTriggerSetMessage.Confirm, [string.Format(LangProvider.Instance.ZoneFormatText, SelectedZone.Id) + string.Format(LangProvider.Instance.TriggerDrSetSuccessMessage, dr),]));
        }
        private void Cancel()
        {
            MessageBus.Default.Send(MsgArgs.From(PopTriggerSetMessage.Cancel, []));
        }

        #region validate methods
        private ValidationResult ValidateDr(int Dr)
        {
            if (Dr < 0 || Dr > 255)
            {
                return new ValidationResult(string.Format(LangProvider.Instance.MustBeInRangeMessage, "0 ~ 256"));
            }

            return ValidationResult.Success;
        }
        #endregion
    }
}
