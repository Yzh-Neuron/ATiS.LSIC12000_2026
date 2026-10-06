namespace LSIC.App.Models
{
    public class Thermostat : ObservableObject
    {
        public int Id { get; }
        public Thermostat(ThermostatSetting setting)
        {
            Id = setting.Id;
            Enable = setting.Enable;
            TargetTemperature = setting.TargetTemperature;
        }

        private bool _enable;
        public bool Enable
        {
            get => _enable;
            set => SetProperty(ref _enable, value);
        }

        private int _targetTemperature;
        public int TargetTemperature
        {
            get => _targetTemperature;
            set => SetProperty(ref _targetTemperature, value);
        }

        private int? _temperature;
        public int? Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }
    }
}