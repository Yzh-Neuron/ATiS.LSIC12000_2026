namespace LSIC.App.Models
{
    /// <summary>
    /// 恒温器（运行时）
    /// </summary>
    public class BurnThermostat : ObservableObject
    {
        public int Id { get; }

        public BurnThermostat(ThermostatSetting setting)
        {
            Id = setting.Id;
            Enable = setting.Enable;
            TargetTemperature = setting.TargetTemperature;
        }

        private bool _enable;
        public bool Enable
        {
            get { return _enable; }
            set { SetProperty(ref _enable, value); }
        }

        private int _targetTemperature;
        public int TargetTemperature
        {
            get { return _targetTemperature; }
            set { SetProperty(ref _targetTemperature, value); }
        }

        private int? _temperature;
        public int? Temperature
        {
            get { return _temperature; }
            set { SetProperty(ref _temperature, value); }
        }

        private int? _kp;
        public int? Kp
        {
            get { return _kp; }
            set { SetProperty(ref _kp, value); }
        }

        private int? _ki;
        public int? Ki
        {
            get { return _ki; }
            set { SetProperty(ref _ki, value); }
        }

        private int? _kd;
        public int? Kd
        {
            get { return _kd; }
            set { SetProperty(ref _kd, value); }
        }
    }
}