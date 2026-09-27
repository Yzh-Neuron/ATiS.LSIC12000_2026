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
                    if (_mode == RunMode.Times)
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
                    RunMode.Time => Time.TotalSeconds,
                    RunMode.Times => Count,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
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
