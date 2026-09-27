namespace LSIC.App.Models
{
    public class Time : ObservableValidator, ICloneable
    {
        public Time()
        {
        }
        public Time(int value, TimeUnit unit)
        {
            Value = value;
            Unit = unit;
        }
        private int _value = 1;
        [ValidationMetadata(Method = nameof(ValidateValue))]
        public int Value
        {
            get => _value;
            set => SetProperty(ref _value, value, true);
        }
        private TimeUnit _unit = TimeUnit.Hour;
        public TimeUnit Unit
        {
            get => _unit;
            set
            {
                if (SetProperty(ref _unit, value))
                {
                    ValidateAllProperties();
                }
            }
        }
        public int TotalSeconds
        {
            get
            {
                return Unit switch
                {
                    TimeUnit.Second => Value,
                    TimeUnit.Minute => Value * 60,
                    TimeUnit.Hour => Value * 60 * 60,
                    TimeUnit.Day => Value * 60 * 60 * 24,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        public override string ToString()
        {
            return $"{Value} {Unit.GetString()}";
        }
        public object Clone()
        {
            return new Time(Value, Unit);
        }
        public void Copy(Time time)
        {
            Value = time.Value;
            Unit = time.Unit;
        }

        #region validate methods
        private ValidationResult ValidateValue(int timeValue)
        {
            const int MaxDays = 3650;
            int max = Unit switch
            {
                TimeUnit.Second => MaxDays * 24 * 60 * 60,
                TimeUnit.Minute => MaxDays * 24 * 60,
                TimeUnit.Hour => MaxDays * 24,
                TimeUnit.Day => MaxDays,
                _ => throw new ArgumentOutOfRangeException()
            };
            return timeValue > 0 && timeValue <= max
                ? ValidationResult.Success
                : new ValidationResult(string.Format(LangProvider.Instance.MustBeInRangMessage, $"0 ~ {max} {Unit.GetString()}"), [LangProvider.Instance.LoopTimeText]);

        }

        #endregion 
    }
}
