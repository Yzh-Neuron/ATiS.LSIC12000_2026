namespace LSIC.App.Models
{
    public class Time : ObservableObject
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
        public int Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        private TimeUnit _unit = TimeUnit.Hour;
        public TimeUnit Unit
        {
            get => _unit;
            set => SetProperty(ref _unit, value);
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
    }
}
