namespace LSIC.App.Models
{
    /// <summary>
    /// 时间范围
    /// </summary>
    public class TimeRange : ObservableObject
    {
        private DateTime _start;
        public DateTime Start
        {
            get => _start;
            set => SetProperty(ref _start, value);
        }

        private DateTime _end;
        public DateTime End
        {
            get => _end;
            set => SetProperty(ref _end, value);
        }
    }
}