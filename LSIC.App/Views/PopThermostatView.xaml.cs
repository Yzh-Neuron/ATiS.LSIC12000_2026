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
            MessageBus.Default.Register(this);
            Loaded += OnLoaded;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Dut_List.SelectedItem != null)
            {
                Dut_List.ScrollIntoView(Dut_List.SelectedItem);
            }
        }
        public MessageResult Receive(MsgArgs<PopThermostatMessage> e)
        {
            switch (e.Message)
            {
                case PopThermostatMessage.Growl:
                    {
                        Growl.Show(new GrowlInfo
                        {
                            Message = e.Args[0] as string,
                            Type = (MessageType)e.Args[1]
                        });
                        break;
                    }
            }
            return MessageResult.Null();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Loaded -= OnLoaded;
            MessageBus.Default.UnregisterAll(this);
        }
    }
}
