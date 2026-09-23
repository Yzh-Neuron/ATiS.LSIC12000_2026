namespace LSIC.App.Models
{

    /// <summary>
    /// 包装多个 DutSetting，用于界面批量设置独立温控。
    /// </summary>
    public class DutSettingWrapper : ObservableObject
    {
        private readonly IReadOnlyList<DutSetting> _dutSettings;
        public bool IsValid { get; }
        public DutSetting DutSettingTemplate { get; }

        private bool? _enable;
        public bool? Enable
        {
            get => _enable;
            set
            {
                if (SetProperty(ref _enable, value) && value.HasValue)
                {
                    ApplyThermostat(t => t.Enable = value.Value);
                }
            }
        }

        private int? _targetTemperature = 250;
        public int? TargetTemperature
        {
            get => _targetTemperature;
            set
            {
                if (SetProperty(ref _targetTemperature, value) && value.HasValue)
                {
                    ApplyThermostat(t => t.TargetTemperature = value.Value);
                }
            }
        }

        public DutSettingWrapper(IEnumerable<DutSetting> dutSettings)
        {
            ArgumentNullException.ThrowIfNull(dutSettings);
            _dutSettings = dutSettings.ToList();

            DutSettingTemplate = _dutSettings.FirstOrDefault(t => t.ThermostatSetting != null);
            IsValid = DutSettingTemplate != null;

            if (!IsValid)
            {
                return;
            }

            if (_dutSettings.All(t => t.ThermostatSetting!.Enable))
            {
                _enable = true;
            }
            else if (_dutSettings.All(t => !t.ThermostatSetting!.Enable))
            {
                _enable = false;
            }
            else
            {
                _enable = null;
            }

            var templateTemp = DutSettingTemplate.ThermostatSetting.TargetTemperature;
            _targetTemperature = _dutSettings.All(t => t.ThermostatSetting!.TargetTemperature == templateTemp) ? templateTemp : null;
        }
        private void ApplyThermostat(Action<ThermostatSetting> action)
        {
            foreach (var setting in _dutSettings)
            {
                if (setting.ThermostatSetting != null)
                {
                    action(setting.ThermostatSetting);
                }
            }
        }
    }
}
