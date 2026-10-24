namespace LSIC.App.ViewModels
{
    public class PopThermostatViewModel : ObservableValidator
    {
        public Zone Zone { get; }
        public BurnDut[] Duts { get; }
        private BurnDut _selectedDut;
        public BurnDut SelectedDut
        {
            get => _selectedDut;
            set
            {
                if (SetProperty(ref _selectedDut, value))
                {
                    SelectedThermostat = value?.BurnThermostat;
                }
            }
        }
        private BurnThermostat _selectedThermostat;
        public BurnThermostat SelectedThermostat
        {
            get => _selectedThermostat;
            set
            {
                if (SetProperty(ref _selectedThermostat, value))
                {
                    Kp = value?.Kp;
                    Ki = value?.Ki;
                    Kd = value?.Kd;
                }
            }
        }
        private double? _kp;
        [ValidationMetadata(Method = nameof(ValidateKp))]
        public double? Kp
        {
            get => _kp;
            set => SetProperty(ref _kp, value, true);
        }
        private double? _ki;
        [ValidationMetadata(Method = nameof(ValidateKi))]
        public double? Ki
        {
            get => _ki;
            set => SetProperty(ref _ki, value, true);
        }
        private double? _kd;
        [ValidationMetadata(Method = nameof(ValidateKd))]
        public double? Kd
        {
            get => _kd;
            set => SetProperty(ref _kd, value, true);
        }
        public ICommand SetPidCommand { get; }
        public PopThermostatViewModel(Zone zone, BurnDut selectedDut)
        {
            Zone = zone;
            Duts = zone.BurnInfo.BurnInBoard.Duts.Where(t => t.BurnThermostat?.Enable == true).OrderBy(d => d.Id).ToArray();
            SelectedDut = Duts.FirstOrDefault(t => t == selectedDut);
            SetPidCommand = new AsyncRelayCommand(SetPidAsync, CanSetPid);

            ValidateAllProperties();
        }
        private bool CanSetPid()
        {
            return !HasErrors && Zone.Connected;
        }
        private async Task SetPidAsync()
        {
            BurnThermostat t = SelectedThermostat;
            await Zone.SetThermostatPIDAsync(t.Id, Kp.Value, Ki.Value, Kd.Value);
            MessageBus.Default.Send(MsgArgs.From(PopThermostatMessage.Growl, [LangProvider.Instance.ThermostatCoefficientSetSuccessMessage, MessageType.Success]));
        }

        #region validate methods
        private ValidationResult ValidateKp(double? kp)
        {
            if (!kp.HasValue || double.IsNaN(kp.Value) || double.IsInfinity(kp.Value) || kp.Value < 0 || kp.Value > 1000)
            {
                return new ValidationResult(string.Format(LangProvider.Instance.MustBeInRangeMessage, "0 ~ 1000"));
            }
            return ValidationResult.Success;
        }
        private ValidationResult ValidateKi(double? ki)
        {
            if (!ki.HasValue || double.IsNaN(ki.Value) || double.IsInfinity(ki.Value) || ki.Value < 0 || ki.Value > 100)
            {
                return new ValidationResult(string.Format(LangProvider.Instance.MustBeInRangeMessage, "0 ~ 100"));
            }
            return ValidationResult.Success;
        }
        private ValidationResult ValidateKd(double? kd)
        {
            if (!kd.HasValue || double.IsNaN(kd.Value) || double.IsInfinity(kd.Value) || kd.Value < 0 || kd.Value > 200)
            {
                return new ValidationResult(string.Format(LangProvider.Instance.MustBeInRangeMessage, "0 ~ 200"));
            }
            return ValidationResult.Success;
        }
        #endregion
    }
}
