
namespace LSIC.App.Views
{
    public partial class WinMainView : IRecipient<MsgArgs<WinMainMessage>>
    {
        public WinMainViewModel ViewModel { get; }
        public WinMainView()
        {
            InitializeComponent();
            ViewModel = new WinMainViewModel();
            DataContext = ViewModel;
            MessageBus.Default.Register(this);
        }
        public MessageResult Receive(MsgArgs<WinMainMessage> e)
        {
            switch (e.Message)
            {
                case WinMainMessage.InputName:
                    {
                        string title = e.Args[0] as string;
                        List<string> filters = e.Args[1] as List<string>;
                        PopInputNameView pop = new PopInputNameView(title, filters);
                        pop.ShowDialog();
                        return MessageResult.From(pop.NameResult);
                    }
                case WinMainMessage.OpenIcEditView:
                    {
                        PopIcEditView pop = new PopIcEditView() { Owner = this };
                        pop.ShowDialog();
                        break;
                    }
                case WinMainMessage.Start:
                    break;
                case WinMainMessage.Pause:
                    break;
                case WinMainMessage.Continue:
                    break;
                case WinMainMessage.Stop:
                    break;
                case WinMainMessage.CleareAlarm:
                    break;
                case WinMainMessage.PatternConvert:
                    break;
                case WinMainMessage.SetTriggerChanne:
                    var rst = MessagePop.Show("文本", MessageType.Question);
                    Debug.WriteLine(rst);
                    break;
                case WinMainMessage.OvenControl:
                    var rst2 = MessagePop.Show("测测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长试下长测测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测测测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文测测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长 测试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长试下长测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本本 测试下长试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本试下长试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本 测试下长文本文本 测试下长文本 测试下长文本 测试下长文本", MessageType.Error);
                    Debug.WriteLine(rst2);
                    break;
                case WinMainMessage.ThermostatMonitor:
                    break;
                case WinMainMessage.Message:
                    break;
                default:
                    break;
            }

            return MessageResult.Null();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MessageBus.Default.UnregisterAll(this);
            Application.Current.Shutdown();
        }

        private void OnAboutButtonClick(object sender, RoutedEventArgs e)
        {

            var pop = new PopAboutView
            {
                Owner = this
            };

            pop.ShowDialog();
        }

    }
}
