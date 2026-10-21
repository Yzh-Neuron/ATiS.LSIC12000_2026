namespace LSIC.App.ViewModels
{
    public class WinMainViewModel : ObservableObject
    {
        private readonly Timer _timer;
        public Device Device { get; }
        private DateTime _now;
        public DateTime Now
        {
            get => _now;
            private set => SetProperty(ref _now, value);
        }
        public WinMainViewModel()
        {
            Device = Device.Instance;
            _timer = new Timer(Tick, null, 0, 1000);
        }
        private void Tick(object _)
        {
            Now = DateTime.Now;
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
