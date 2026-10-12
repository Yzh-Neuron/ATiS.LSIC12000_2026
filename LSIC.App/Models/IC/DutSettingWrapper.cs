namespace LSIC.App.Models
{
    /// <summary>
    /// 包装多个 DutSetting，用于界面批量设置独立温控。
    /// </summary>
    public class DutSettingWrapper : ObservableValidator
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
                    ValidateProperty(nameof(TargetTemperature));
                    ApplyThermostat(t => t.Enable = value.Value);
                }
            }
        }

        private int? _targetTemperature = 250;
        [ValidationMetadata(Method = nameof(ValidateTargetTemperature))]
        public int? TargetTemperature
        {
            get => _targetTemperature;
            set
            {
                if (SetProperty(ref _targetTemperature, value, true) && value.HasValue)
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
                DutSettingTemplate = _dutSettings.FirstOrDefault();
                return;
            }

            if (_dutSettings.All(t => t.ThermostatSetting?.Enable == true))
            {
                _enable = true;
            }
            else if (_dutSettings.All(t => t.ThermostatSetting?.Enable == false))
            {
                _enable = false;
            }
            else
            {
                _enable = null;
            }

            var templateTemp = DutSettingTemplate.ThermostatSetting.TargetTemperature;
            _targetTemperature = _dutSettings.All(t => t.ThermostatSetting?.TargetTemperature == templateTemp) ? templateTemp : null;

            ValidateAllProperties();
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

        #region validate methods
        private ValidationResult ValidateTargetTemperature(int? targetTemperature)
        {
            if (targetTemperature == null || Enable == null || Enable != true)
            {
                return ValidationResult.Success;
            }

            Device.Instance.SystemConfig.ThermostatInfo.TempRange.TryGetValue(0, out Limit limit);
            if (!limit.Contains(targetTemperature.Value))
            {
                return new ValidationResult(string.Format(LangProvider.Instance.MustBeInRangMessage, limit.GetRangString(10, "", 10, "℃")), [LangProvider.Instance.ThermostatIndexText]);
            }
            return ValidationResult.Success;
        }

        #endregion 
    }
}
