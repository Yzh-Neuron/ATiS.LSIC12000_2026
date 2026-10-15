namespace LSIC.App.Models
{
    public class DpsSetting : ObservableValidator, ICloneable
    {
        private DpsInfo _dpsInfo;
        public DpsInfo DpsInfo
        {
            get => _dpsInfo;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                if (SetProperty(ref _dpsInfo, value))
                {
                    Id = value.Id;
                    Name = value.Name;
                    ValidateAllProperties();
                }
            }
        }
        public int Id { get; private set; }
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private int _vol;
        [ValidationMetadata(Method = nameof(ValidateVol))]
        public int Vol
        {
            get => _vol;
            set => SetProperty(ref _vol, value, true);
        }

        private int _uvp;
        [ValidationMetadata(Method = nameof(ValidateUvp))]
        public int Uvp
        {
            get => _uvp;
            set => SetProperty(ref _uvp, value, true);
        }

        private int _ovp;
        [ValidationMetadata(Method = nameof(ValidateOvp))]
        public int Ovp
        {
            get => _ovp;
            set => SetProperty(ref _ovp, value, true);
        }

        private int _ocp;
        [ValidationMetadata(Method = nameof(ValidateOcp))]
        public int Ocp
        {
            get => _ocp;
            set => SetProperty(ref _ocp, value, true);
        }

        private int _raise;
        [ValidationMetadata(Method = nameof(ValidateRaise))]
        public int Raise
        {
            get => _raise;
            set => SetProperty(ref _raise, value, true);
        }

        private int _delay;
        [ValidationMetadata(Method = nameof(ValidateDelay))]
        public int Delay
        {
            get => _delay;
            set => SetProperty(ref _delay, value, true);
        }

        public DpsSetting(DpsInfo dpsInfo)
        {
            Vol = dpsInfo.VolDefault;
            Delay = dpsInfo.DelayDefault;
            Raise = dpsInfo.RiseDefault;
            DpsInfo = dpsInfo;
        }
        public object Clone()
        {
            var setting = new DpsSetting(DpsInfo)
            {
                Vol = Vol,
                Uvp = Uvp,
                Ovp = Ovp,
                Ocp = Ocp,
                Raise = Raise,
                Delay = Delay
            };
            return setting;
        }
        public void Copy(DpsSetting copy)
        {
            Vol = copy.Vol;
            Uvp = copy.Uvp;
            Ovp = copy.Ovp;
            Ocp = copy.Ocp;
            Raise = copy.Raise;
            Delay = copy.Delay;
            DpsInfo = copy.DpsInfo;
        }

        #region validate methods

        private ValidationResult ValidateVol(int arg)
        {
            if (DpsInfo?.VolRange.TryGetValue(0, out Limit limit) == true && !limit.Contains(arg))
            {
                return new ValidationResult(string.Format(LangProvider.Instance.MustBeInRangeMessage, limit.GetRangString(1000, "", 1000, "V")), [LangProvider.Instance.SetVoltageText]);
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateUvp(int arg)
        {
            if (DpsInfo?.VolLimitRange.TryGetValue(0, out Limit limit) == true && !limit.Contains(arg))
            {
                return new ValidationResult(string.Format(LangProvider.Instance.MustBeInRangeMessage, limit.GetRangString(1000, "", 1000, "V")), [LangProvider.Instance.VoltageFloorText]);
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateOvp(int arg)
        {
            if (DpsInfo?.VolLimitRange.TryGetValue(0, out Limit limit) == true && !limit.Contains(arg))
            {
                return new ValidationResult(string.Format(LangProvider.Instance.MustBeInRangeMessage, limit.GetRangString(1000, "", 1000, "V")), [LangProvider.Instance.VoltageCeilingText]);
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateOcp(int arg)
        {
            if (DpsInfo?.CurRange.TryGetValue(0, out Limit limit) == true && !limit.Contains(arg))
            {
                return new ValidationResult(string.Format(LangProvider.Instance.MustBeInRangeMessage, limit.GetRangString(1000, "", 1000, "A")), [LangProvider.Instance.CurrentCeilingText]);
            }
            return ValidationResult.Success;
        }

        private ValidationResult ValidateRaise(int arg)
        {
            if (DpsInfo?.RiseRange.TryGetValue(0, out Limit limit) == true && !limit.Contains(arg))
            {
                return new ValidationResult(string.Format(LangProvider.Instance.MustBeInRangeMessage, limit.GetRangString(1000, "", 1000, "s")), [LangProvider.Instance.RaiseTimeText]);
            }
            return ValidationResult.Success;
        }

        private ValidationResult ValidateDelay(int arg)
        {
            if (DpsInfo?.DelayRange.TryGetValue(0, out Limit limit) == true && !limit.Contains(arg))
            {
                return new ValidationResult(string.Format(LangProvider.Instance.MustBeInRangeMessage, limit.GetRangString(1000, "", 1000, "s")), [LangProvider.Instance.DelayText]);
            }
            return ValidationResult.Success;
        }
        #endregion
    }
}