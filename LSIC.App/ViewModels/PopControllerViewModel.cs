namespace LSIC.App.ViewModels
{
    public class PopControllerViewModel : ObservableValidator
    {
        public Device Device { get; }
        public Zone[] Zones { get; }
        private Zone _selectedZone;
        public Zone SelectedZone
        {
            get => _selectedZone;
            set => SetProperty(ref _selectedZone, value);
        }
        private string _valveControl;
        [ValidationMetadata(Method = nameof(ValidateValveControl))]
        public string ValveControl
        {
            get => _valveControl;
            set => SetProperty(ref _valveControl, value, true);
        }
        public Controller Controller { get; }
        public ICommand ToggleAlarmLightCommand { get; }
        public ICommand DoorOpenCommand { get; }
        public ICommand DoorCloseCommand { get; }
        public ICommand ApplyValveControlCommand { get; }
        public PopControllerViewModel()
        {
            Device = Device.Instance;
            Zones = Device.Zones;
            SelectedZone = Zones?.FirstOrDefault();
            Controller = Device.Controller;

            ToggleAlarmLightCommand = new AsyncRelayCommand<LightAlarmState>(ToggleAlarmLightAsync, CanToggleAlarmLight);
            DoorOpenCommand = new AsyncRelayCommand(DoorOpenAsync, CanDoorOpen);
            DoorCloseCommand = new AsyncRelayCommand(DoorCloseAsync, CanDoorClose);
            ApplyValveControlCommand = new AsyncRelayCommand(ApplyValveControlAsync, CanApplyValveControl);

            ValidateAllProperties();
        }
        private bool CanDoorClose()
        {
            return Controller?.Connected == true;
        }
        private async Task DoorCloseAsync()
        {
            await Controller.SendDoorControlAsync(false);
        }
        private bool CanDoorOpen()
        {
            return Controller?.Connected == true;
        }
        private async Task DoorOpenAsync()
        {
            await Controller.SendDoorControlAsync(true);
        }
        private bool CanToggleAlarmLight(LightAlarmState obj)
        {
            return Controller?.Connected == true;
        }
        private async Task ToggleAlarmLightAsync(LightAlarmState flag)
        {
            LightAlarmState current = Controller.LightAlarm ?? 0;
            LightAlarmState next = current.HasFlag(flag) ? current & ~flag : current | flag;
            await Controller.SendAlarmLightControlAsync(next);
        }
        private bool CanApplyValveControl()
        {
            if (SelectedZone != null && Controller?.Connected == true && HasErrors == false)
            {
                return true;
            }
            return false;
        }
        private async Task ApplyValveControlAsync()
        {
            if (double.TryParse(ValveControl, out double val) || val < 0 || val > 100)
            {
                await Controller.SendWaterValveControlAsync((byte)SelectedZone.Id, (uint)val * 10);
            }
        }

        #region validate methods
        private ValidationResult ValidateValveControl(string valve)
        {
            if (!double.TryParse(valve, out double val) || val < 0 || val > 100)
            {
                return new ValidationResult(string.Format(LangProvider.Instance.MustBeInRangeMessage, $"0.0% ~ 100.0%"));
            }
            return ValidationResult.Success;
        }
        #endregion
    }
}
