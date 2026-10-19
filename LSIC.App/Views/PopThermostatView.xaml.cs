namespace LSIC.App.Views
{
    public partial class PopThermostatView : IRecipient<MsgArgs<PopThermostatMessage>>
    {
        public PopThermostatViewModel ViewModel { get; }
        public PopThermostatView(Zone zone, BurnDut selectedDut)
        {
            ViewModel = new PopThermostatViewModel(zone, selectedDut);
            DataContext = ViewModel;
            InitializeComponent();
            Loaded += PopThermostatView_Loaded;
        }
        private void PopThermostatView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Dut_List.SelectedItem!=null)
            {
                Dut_List.ScrollIntoView(Dut_List.SelectedItem);
            }
        }
        public MessageResult Receive(MsgArgs<PopThermostatMessage> e)
        {
            switch (e.Message)
            {
                case PopThermostatMessage.SetPid:
                    {


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
            Loaded -= PopThermostatView_Loaded;
            MessageBus.Default.UnregisterAll(this);
        }
    }
}