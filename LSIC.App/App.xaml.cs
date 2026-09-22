using System.Windows.Threading;

namespace LSIC.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;

            //注册语言模块
            GlobalLanguageManager.RegisterLanguageResources(UiLanguage.Chinese, [
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/zh-CN/Message.xaml")},
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/zh-CN/UI.xaml")},
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/en-US/Enum.xaml")}
            ]);

            GlobalLanguageManager.RegisterLanguageResources(UiLanguage.English, [
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/en-US/Message.xaml")},
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/en-US/UI.xaml")},
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/en-US/Enum.xaml")}
            ]);
        }
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            MessagePop.Show(e.Exception.ToString(), MessageType.Error);
            //LogUtil.Error("DispatcherUnhandledException", e.Exception);
        }

        private void OnTaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            MessagePop.Show(e.Exception.ToString(), MessageType.Error);
            //LogUtil.Error("TaskSchedulerUnobservedTaskException", e.Exception);
        }

        private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessagePop.Show(e.ExceptionObject.ToString(), MessageType.Error);
            //LogUtil.Error("CurrentDomainUnhandledException", e.ExceptionObject as Exception);
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // 解析配置文件
            try
            {
                SystemConfig config = new SystemConfig();

                bool successConfig = Ini.TryParse("Config.ini", Encoding.UTF8, ref config, out int errorLineConfig);
                if (!successConfig)
                {
                    throw new Exception(errorLineConfig.ToString());
                }
                Device.Initialize(config);
            }
            catch (Exception ex)
            {
                string error = string.Format(LangProvider.Instance.DeviceConfigParseFailureMessage, ex.Message);
                MessagePop.Show(error, MessageType.Error);
                Shutdown();
                return;
            }
#if DEBUG
            new WinMainView().Show();
            //new PopIcEditView().Show();
#else
            new WinMainView().Show();
#endif
        }
    }

}
