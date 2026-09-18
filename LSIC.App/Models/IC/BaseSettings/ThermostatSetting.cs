namespace LSIC.App.Models
{
    public class ThermostatSetting : ObservableObject
    {
        public int Id { get; private set; }

        private bool _enable;
        public bool Enable
        {
            get => _enable;
            set => SetProperty(ref _enable, value);
        }

        private int _targetTemperature = 250;
        public int TargetTemperature
        {
            get => _targetTemperature;
            set => SetProperty(ref _targetTemperature, value);
        }

        public static ThermostatSetting Create(int id)
        {
            return new ThermostatSetting { Id = id };
        }
    }
}
