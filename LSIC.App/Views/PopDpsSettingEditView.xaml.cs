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
                default:
                    {
                        break;
                    }
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
