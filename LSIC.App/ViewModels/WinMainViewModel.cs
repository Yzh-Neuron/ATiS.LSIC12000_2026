namespace LSIC.App.ViewModels
{
    public class WinMainViewModel : ObservableObject
    {
        private readonly Timer _timer;
        private DateTime _now;
        public DateTime Now
        {
            get => _now;
            private set => SetProperty(ref _now, value);
        }
        public ICommand OpenIcEditViewCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand ContinueCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand CloseAlarmCommand { get; private set; }
        public ICommand PatternConvertCommand { get; private set; }
        public ICommand SetTriggerChanneCommand { get; private set; }
        public ICommand ThermostatMonitorCommand { get; private set; }
        public WinMainViewModel()
        {
            OpenIcEditViewCommand = new RelayCommand(OpenIcEditView);
            StartCommand = new AsyncRelayCommand(Start);
            PauseCommand = new AsyncRelayCommand(Pause);
            ContinueCommand = new AsyncRelayCommand(Continue);
            StopCommand = new AsyncRelayCommand(Stop);
            CloseAlarmCommand = new AsyncRelayCommand(CloseAlarm);
            PatternConvertCommand = new AsyncRelayCommand(PatternConvert);
            SetTriggerChanneCommand = new AsyncRelayCommand(SetTriggerChanne);
            ThermostatMonitorCommand = new AsyncRelayCommand(ThermostatMonitor);
            _timer = new Timer(Tick, null, 0, 1000);
        }

        /// <summary>
        /// 打开器件编辑视图
        /// </summary>
        private void OpenIcEditView()
        {
            MessageBus.Default.Send(MsgArgs.From(WinMainMessage.OpenIcEditView, []));
        }

        private async Task Start()
        {
            MessageBus.Default.Send(MsgArgs.From(WinMainMessage.Start, []));
        }
        private async Task Pause()
        {
            MessageBus.Default.Send(MsgArgs.From(WinMainMessage.Pause, []));
        }
        private async Task Continue()
        {
            MessageBus.Default.Send(MsgArgs.From(WinMainMessage.Continue, []));
        }
        private async Task Stop()
        {
            MessageBus.Default.Send(MsgArgs.From(WinMainMessage.Stop, []));
        }
        private async Task CloseAlarm()
        {
            //await Device.Instance.HVControlBoard.CloseAlarmAsync();
        }
        private async Task PatternConvert()
        {
            MessageBus.Default.Send(MsgArgs.From(WinMainMessage.PatternConvert, []));
        }
        private async Task SetTriggerChanne()
        {
            MessageBus.Default.Send(MsgArgs.From(WinMainMessage.SetTriggerChanne, []));
        }
        private async Task ThermostatMonitor()
        {
            MessageBus.Default.Send(MsgArgs.From(WinMainMessage.ThermostatMonitor, []));
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
