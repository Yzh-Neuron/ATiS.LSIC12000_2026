namespace LSIC.App.ViewModels
{
    public class CtrBurnInfoViewModel : ObservableValidator
    {
        private Zone _zone;
        public Zone Zone
        {
            get => _zone;
            set => SetProperty(ref _zone, value);
        }
        public ICommand SendIcCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand ClearBurnInfoCommand { get; private set; }
        public CtrBurnInfoViewModel()
        {
            SendIcCommand = new AsyncRelayCommand(SendIcAsync);
            StartCommand = new AsyncRelayCommand(StartAsync);
            StopCommand = new AsyncRelayCommand(StopAsync);
            ClearBurnInfoCommand = new RelayCommand(ClearBurnInfo);
        }
        private async Task SendIcAsync()
        {
            await Zone.SendIcAsync();
        }
        private async Task StartAsync()
        {
            var msgResult = MessageBus.Default.Send(MsgArgs.From(WinMainMessage.DutMask, [])).First();

            if (msgResult.Result is byte[] dutMask)
            {
                await Zone.StartBurnAsync(dutMask);
            }
        }
        private async Task StopAsync()
        {
            await Zone.StopBurnAsync();
        }
        private void ClearBurnInfo()
        {
            Zone.ResetStateToIdle();
        }

    }
}
