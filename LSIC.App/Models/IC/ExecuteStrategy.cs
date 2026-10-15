namespace LSIC.App.Models
{
    /// <summary>
    /// 执行策略
    /// </summary>
    public class ExecuteStrategy : ObservableValidator
    {
        public ExecuteStrategy()
        {
            ValidateAllProperties();
        }
        private ExecuteMode _mode;
        public ExecuteMode Mode
        {
            get => _mode;
            set
            {
                if (SetProperty(ref _mode, value))
                {
                    if (_mode == ExecuteMode.Times)
                    {
                        Time = new Time();
                    }
                    else
                    {
                        Count = 1;
                    }
                }
            }
        }
        private Time _time = new();
        public Time Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }
        private int _count = 1;
        [ValidationMetadata(Method = nameof(ValidateCount))]
        public int Count
        {
            get => _count;
            set => SetProperty(ref _count, value, true);
        }
        public int TotalSecondsOrCount
        {
            get
            {
                return Mode switch
                {
                    ExecuteMode.Time => Time.TotalSeconds,
                    ExecuteMode.Times => Count,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        public object Clone()
        {
            var executeStrategy = new ExecuteStrategy()
            {
                Mode = Mode,
                Time = (Time)Time.Clone(),
                Count = Count,
            };
            return executeStrategy;
        }
        public void Copy(ExecuteStrategy executeStrategy)
        {
            Mode = executeStrategy.Mode;
            Count = executeStrategy.Count;
            Time.Copy(executeStrategy.Time);
        }

        #region validate methods
        private ValidationResult ValidateCount(int Count)
        {
            if (Count <= 0|| Count> 2000000000)
            {
                new ValidationResult(string.Format(LangProvider.Instance.MustBeInRangeMessage, $"1 ~ 2000000000 {LangProvider.Instance.TimesText}"));
            }
            return ValidationResult.Success;
        }

        #endregion 
    }
}
