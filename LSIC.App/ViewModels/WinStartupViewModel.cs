namespace LSIC.App.ViewModels
{
    public class WinStartupViewModel : ObservableObject
    {
        public Device Device { get; }
        public ObservableCollection<CheckItem> CheckItems { get; } = new ObservableCollection<CheckItem>();
        public WinStartupViewModel()
        {
            Device = Device.Instance;
            CheckConnectionsAsync();
        }
        private void CheckConnectionsAsync()
        {
            Task.Run(async () =>
            {
                foreach (Zone zone in Device.Zones)
                {
                    zone.Startup();
                }

#if DEBUG
                await Task.Delay(2000);
                this.InvokeOnUI(() => MessageBus.Default.Send(MsgArgs.From(WinStartupMessage.Initialized, [])));
                return;
#endif
                foreach (Zone zone in Device.Zones)
                {
                    string name = string.Format(LangProvider.Instance.ConnectionCheckingText, zone.Id);
                    CheckItem item = new CheckItem(name);
                    this.InvokeOnUI(() => CheckItems.Add(item));
                    int times = 0;
                    while (times < 30)
                    {
                        if (!zone.Connected)
                        {
                            await Task.Delay(10);
                            times++;
                            continue;
                        }
                        break;
                    }
                    name = zone.Connected ? string.Format(LangProvider.Instance.ConnectSuccessText, zone.Id) : string.Format(LangProvider.Instance.ConnectFailureText, zone.Id);
                    item.Name = name;
                    item.Connected = zone.Connected;
                }

                CheckItem init = new CheckItem(LangProvider.Instance.InitText);
                this.InvokeOnUI(() => CheckItems.Add(init));
                await Task.Delay(1000);
                this.InvokeOnUI(() => MessageBus.Default.Send(MsgArgs.From(WinStartupMessage.Initialized, [])));
            });
        }
    }
}
