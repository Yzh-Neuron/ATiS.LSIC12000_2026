namespace LSIC.App.ViewModels
{
    public class WinStartupViewModel : ObservableObject
    {
        public WinStartupViewModel()
        {

        }



        public ObservableCollection<CheckItem> CheckItems { get; } = new ObservableCollection<CheckItem>();


        public async Task StartCheck()
        {
            await Task.Delay(3000);
            MessageBus.Default.Send(MsgArgs.From(WinStartupMessage.Initialized, []));
        }
    }
}
