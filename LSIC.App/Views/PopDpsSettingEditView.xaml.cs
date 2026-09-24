namespace LSIC.App.Views
{
    public partial class PopDpsSettingEditView : IRecipient<MsgArgs<PopDpsSettingEditMessage>>
    {
        public DpsSetting DpsSetting { get; private set; }
        public PopDpsSettingEditViewModel ViewModel { get; }
        public PopDpsSettingEditView(IEnumerable<DpsInfo> dpsInfos, DpsSetting dpsSetting)
        {
            InitializeComponent();
            ViewModel = new PopDpsSettingEditViewModel(dpsInfos, dpsSetting);
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }

        public MessageResult Receive(MsgArgs<PopDpsSettingEditMessage> e)
        {
            switch (e.Message)
            {
                case PopDpsSettingEditMessage.Confirm:
                    {
                        DpsSetting = e.Args[0] as DpsSetting;
                        Close();
                        break;
                    }
                case PopDpsSettingEditMessage.Cancel:
                    {
                        DpsSetting = null;
                        Close();
                        break;
                    }
                case PopDpsSettingEditMessage.ShowValidateError:
                    {
                        List<ValidationResult> errors = e.Args[0] as List<ValidationResult>;
                        MessagePop.Show(errors.ToMessageString(), MessageType.Error);
                        break;
                    }
                default:
                    break;
            }

            return MessageResult.Null();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MessageBus.Default.UnregisterAll(this);
        }
    }
}

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
            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
        }
        private void Confirm()
        {
            List<ValidationResult> errors = DpsSetting.GetAllErrorsRecursive();
            if (errors.Count > 0)
            {
                MessageBus.Default.Send(MsgArgs.From(PopDpsSettingEditMessage.ShowValidateError, [errors]));
            }
            else
            {
                MessageBus.Default.Send(MsgArgs.From(PopDpsSettingEditMessage.Confirm, [DpsSetting]));
            }

        }
        private void Cancel()
        {
            MessageBus.Default.Send(MsgArgs.From(PopDpsSettingEditMessage.Cancel, []));
        }

    }
}

namespace LSIC.App.ViewMessages
{
    public enum PopDpsSettingEditMessage
    {
        Confirm,
        Cancel,
        ShowValidateError
    }
}
