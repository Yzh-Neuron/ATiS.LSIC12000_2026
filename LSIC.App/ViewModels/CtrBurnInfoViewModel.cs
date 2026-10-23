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
            try
            {
                await Zone.SendIcAsync();
            }
            catch (Exception ex)
            {
                MessageBus.Default.Send(MsgArgs.From(WinMainMessage.Growl, [string.Format(LangProvider.Instance.SendIcFailureMessage, ex.Message), MessageType.Error]));
            }
        }
        private async Task StartAsync()
        {
            try
            {
                byte[] dutMask = null;

                if (Zone.Pluged)
                {
                    var msgResult = MessageBus.Default.Send(MsgArgs.From(WinMainMessage.DutMask, [Zone.BurnInfo.BurnInBoard])).First();

                    if (msgResult.Result is byte[] d)
                    {
                        dutMask = d;
                    }

                    if (dutMask is null)
                    {
                        return;
                    }
                }

                await Zone.StartBurnAsync(dutMask);
            }
            catch (Exception ex)
            {
                MessageBus.Default.Send(MsgArgs.From(WinMainMessage.Growl, [string.Format(LangProvider.Instance.BurnStartFailureMessage, ex.Message), MessageType.Error]));
            }
        }
        private async Task StopAsync()
        {
            try
            {
                await Zone.StopBurnAsync();
            }
            catch (Exception ex)
            {
                MessageBus.Default.Send(MsgArgs.From(WinMainMessage.Growl, [string.Format(LangProvider.Instance.BurnStopFailureMessage, ex.Message), MessageType.Error]));
            }
        }
        private void ClearBurnInfo()
        {
            Zone.ResetStateToIdle();
        }
    }
}
