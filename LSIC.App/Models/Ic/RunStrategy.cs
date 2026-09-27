namespace LSIC.App.Models
{
    /// <summary>
    /// 运行策略
    /// </summary>
    public class RunStrategy : ObservableValidator
    {
        private RunMode _mode;
        public RunMode Mode
        {
            get => _mode;
            set
            {
                if (SetProperty(ref _mode, value))
                {
                    OnPropertyChanged(nameof(Info));
                }
            }
        }
        private Time _time;
        public Time Time
        {
            get => _time;
            set
            {
                if (SetProperty(ref _time, value))
                {
                    OnPropertyChanged(nameof(Info));
                }
            }
        }
        private int _count = 1;
        public int Count
        {
            get => _count;
            set
            {
                if (SetProperty(ref _count, value,true))
                {
                    OnPropertyChanged(nameof(Info));
                }
            }
        }
        public int TotalSecondsOrCount
        {
            get
            {
                return Mode switch
                {
                    RunMode.Time => Time.TotalSeconds,
                    RunMode.Times => Count,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        public string Info
        {
            get
            {
                return Mode switch
                {
                    RunMode.Time => Time.ToString(),
                    RunMode.Times => $"{Count} {LangProvider.Instance.TimesText}",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        public RunStrategy()
        {
            Time = new Time();
        }
        public object Clone()
        {
            var runStrategy = new RunStrategy()
            {
                Mode = Mode,
                Time = (Time)Time.Clone(),
                Count = Count,
            };
            return runStrategy;
        }
        public void Copy(RunStrategy runStrategy)
        {
            Mode = runStrategy.Mode;
            Count = runStrategy.Count;
            Time.Copy(runStrategy.Time);
        }
    }
}
