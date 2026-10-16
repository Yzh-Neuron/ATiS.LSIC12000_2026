using System.Security.Cryptography;

namespace LSIC.App.Views
{
    public partial class WinMainView : IRecipient<MsgArgs<WinMainMessage>>
    {
        private const string PublicKey = "<RSAKeyValue><Modulus>6rnqXdAXYIgKCBIw2sfC5KWu3av0L9SQc7JdG9hWDIl/uKzPss4XyrdJkG/jXnUCODMukaSweLiAiDJzIHVYKJbDCoa5ul2YTYSlHm3SzzM2l1euIyNKlp7al+xWVQRImF73hDPtaOc8jHIDWWh8vT809tlCHM8Ix9GBXwgrKL0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
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
                case WinMainMessage.Growl:
                    {
                        Growl.Show(new GrowlInfo
                        {
                            Message = e.Args[0] as string,
                            Type = (MessageType)e.Args[1]
                        });
                        break;
                    }
                case WinMainMessage.ZoneListView:
                    {
                        new PopZoneListView { Owner = this }.ShowDialog();
                        break;
                    }
                case WinMainMessage.DutMask:
                    {
                        BurnInBoard board = e.Args[0] as BurnInBoard;
                        PopDutMaskView pop = new PopDutMaskView(board) { Owner = this };
                        pop.ShowDialog();
                        return MessageResult.From(pop.DutMask);
                    }
                default:
                    {
                        break;
                    }
            }

            return MessageResult.Null();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            MessageBus.Default.UnregisterAll(this);
            MessageBus.Default.UnregisterAll(LogBrowse);
            Application.Current.Shutdown();
        }
        private void OnAboutButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            new PopAboutView { Owner = this }.ShowDialog();
        }
        private void OnDeviceInfoButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            new PopDeviceInfoView { Owner = this }.ShowDialog();
        }
        private void OnUpgradeButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            new PopUpgradeView { Owner = this }.ShowDialog();
        }
        private void OnDeviceCheckButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            new PopDeviceCheckView { Owner = this }.ShowDialog();
        }
        private void OnDpsCheckButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            new PopDpsCheckView { Owner = this }.ShowDialog();
        }
        private void OnOpenIcEditButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            WindowUtility.ShowOrActivate<PopIcEditView>();
        }
        private void OnPatternConvertButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            string content = "{" + "LSIC120000" + "|" + DateTime.Now.ToString("yyyyMMddHHmmss") + "|" + (LangProvider.Instance.CurrentLanguage == UiLanguage.English ? "en-us" : "zh-cn") + "}";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
            rsa.FromXmlString(PublicKey);
            byte[] data = rsa.Encrypt(Encoding.ASCII.GetBytes(content), false);
            string encrypted = Convert.ToBase64String(data);
            System.Diagnostics.Process process = new();
            ProcessStartInfo startInfo = new ProcessStartInfo(".\\HK.Pattern.App.exe")
            {
                Arguments = encrypted,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            process.StartInfo = startInfo;
            process.Start();

            JobUtility.Instance.AddProcess(process.Handle);
        }
        private void OnSetTriggerChannelButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            PopTriggerSetView pop = new PopTriggerSetView()
            {
                Owner = this
            };
            pop.ShowDialog();

            if (pop.SuccessMessage is not null)
            {
                Growl.Show(new GrowlInfo
                {
                    Message = pop.SuccessMessage,
                    Type = MessageType.Success,
                });
            }
        }
        private void OnProductionModeMenuClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (Device.Instance.Mode != DeviceMode.Production)
            {
                Device.Instance.Mode = DeviceMode.Production;
                Growl.Info(DeviceMode.Production.GetString());
            }
        }
        private void OnEngineeringModeMenuClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (Device.Instance.Mode != DeviceMode.Engineering)
            {
                Device.Instance.Mode = DeviceMode.Engineering;
                Growl.Info(DeviceMode.Engineering.GetString());
            }
        }
        private void OnAdministratorModeMenuClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (Device.Instance.Mode != DeviceMode.Administrator)
            {
                PopPermissionVerifyView pop = new PopPermissionVerifyView()
                {
                    Owner = this
                };
                if (pop.ShowDialog() == true)
                {
                    Device.Instance.Mode = DeviceMode.Administrator;
                    Growl.Info(DeviceMode.Administrator.GetString());
                }
            }
        }
        private void OnLanguageChineseClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Device.Instance.Language = UiLanguage.Chinese;
        }
        private void OnLanguageEnglishClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Device.Instance.Language = UiLanguage.English;
        }
    }
}
