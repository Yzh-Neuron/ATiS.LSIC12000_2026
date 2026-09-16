namespace LSIC.App.ViewModels
{
    public class WinStartupViewModel : ObservableObject
    {
        public Guid Guid { get; } = Guid.NewGuid();
        public WinStartupViewModel()
        {

        }

        public ObservableCollection<CheckItem> CheckItems { get; } = new ObservableCollection<CheckItem>();


        public async Task StartCheck()
        {
            await MessageBus.Default.SendAsync(MsgArgs.From(WinStartupMessage.Initialized, []), Guid);
        }
    }
}
