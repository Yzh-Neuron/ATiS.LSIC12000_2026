namespace LSIC.App.ViewModels
{
    public class WinStartupViewModel : ObservableObject
    {
        public Device Device { get; } = Device.Instance;
        public ObservableCollection<CheckItem> CheckItems { get; } = new ObservableCollection<CheckItem>();
        public WinStartupViewModel()
        {
            Device = Device.Instance;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerAsync();

        }
        private async void DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (Zone zone in Device.Zones)
            {
                zone.Startup();
            }

            foreach (Zone zone in Device.Zones)
            {
                string name = string.Format(LangProvider.Instance.ConntectionCheckingMessage, zone.Id);
                CheckItem item = new CheckItem(name);
                this.InvokeOnUI(() => CheckItems.Add(item));
                int times = 0;
                while (times < 30)
                {
                    if (!zone.Connected)
                    {
                        await Task.Delay(10);
                        times++;
                    }
                }
                name = zone.Connected ? string.Format(LangProvider.Instance.ConntectSuccessMessage, zone.Id) : string.Format(LangProvider.Instance.ConntectFailureMessage, zone.Id);
                item.Name = name;
                item.Connected = zone.Connected;
            }
            CheckItem init = new CheckItem(LangProvider.Instance.InitMessage);
            this.InvokeOnUI(() => CheckItems.Add(init));
            await Task.Delay(1000);
            this.InvokeOnUI(() => MessageBus.Default.Send(MsgArgs.From(WinStartupMessage.Initialized, [])));
        }
    }
}
