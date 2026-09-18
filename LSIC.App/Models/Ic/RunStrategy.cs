namespace LSIC.App.Models
{
    /// <summary>
    /// 运行策略
    /// </summary>
    public class RunStrategy : ObservableObject
    {
        public RunStrategy()
        {
            Time = new Time();
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

        private RunMode _mode;
        public RunMode Mode
        {
            get => _mode;
            set => SetProperty(ref _mode, value);
        }

        private Time _time;
        public Time Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        private int _count = 1;
        public int Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
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

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName is nameof(Mode) or nameof(Time) or nameof(Count))
            {
                OnPropertyChanged(nameof(Info));
            }
        }
    }
}
