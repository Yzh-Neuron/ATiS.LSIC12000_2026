namespace LSIC.App.Models
{
    public class ThermostatSetting : ObservableValidator
    {
        public int Id { get; private set; }

        private bool _enable;
        public bool Enable
        {
            get => _enable;
            set
            {

                if (SetProperty(ref _enable, value))
                {
                    ValidateProperty(nameof(TargetTemperature));
                }
            }
        }

        private int _targetTemperature = 250;
        [ValidationMetadata(Method = nameof(ValidateTargetTemperature))]
        public int TargetTemperature
        {
            get => _targetTemperature;
            set => SetProperty(ref _targetTemperature, value, true);
        }

        public ThermostatSetting(int id)
        {
            Id = id;
        }


        #region validate methods
        private ValidationResult ValidateTargetTemperature(int targetTemperature)
        {
            if (!Enable)
            {
                return ValidationResult.Success;
            }

            Device.Instance.SystemConfig.ThermostatInfo.TempRange.TryGetValue(0, out Limit limit);
            if (!limit.Contains(targetTemperature))
            {
                return new ValidationResult(string.Format(LangProvider.Instance.MustBeInRangMessage, limit.GetRangString(10, "", 10, "℃")), [LangProvider.Instance.ThermostatIndexText + Id]);
            }
            return ValidationResult.Success;
        }

        #endregion 
    }
}
