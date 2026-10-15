namespace LSIC.App.ViewModels
{
    public class PopDpsSettingEditViewModel : ObservableObject
    {
        public IEnumerable<DpsInfo> DpsInfos { get; private set; }
        public DpsSetting DpsSetting { get; private set; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }
        public PopDpsSettingEditViewModel(IEnumerable<DpsInfo> dpsInfos, DpsSetting dpsSetting)
        {
            DpsInfos = dpsInfos;
            DpsSetting = dpsSetting;
            ConfirmCommand = new RelayCommand(Confirm, CanConfirm);
            CancelCommand = new RelayCommand(Cancel);
        }
        private bool CanConfirm()
        {
            return DpsSetting.GetAllExistingErrorsRecursive().Count == 0;
        }
        private void Confirm()
        {
            MessageBus.Default.Send(MsgArgs.From(PopDpsSettingEditMessage.Confirm, [DpsSetting]));
        }
        private void Cancel()
        {
            MessageBus.Default.Send(MsgArgs.From(PopDpsSettingEditMessage.Cancel, []));
        }
    }
}
