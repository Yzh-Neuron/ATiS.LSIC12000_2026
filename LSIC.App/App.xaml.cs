using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace LSIC.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 60 });
            ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata { DefaultValue = 250 });
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata { DefaultValue = 6000 });
        }
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
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/zh-CN/Enum.xaml")},
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/zh-CN/Log.xaml")},
            ]);

            GlobalLanguageManager.RegisterLanguageResources(UiLanguage.English, [
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/en-US/Message.xaml")},
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/en-US/UI.xaml")},
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/en-US/Enum.xaml")},
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/en-US/Log.xaml")},
            ]);
        }
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            string message = string.Format(LangProvider.Instance.UnhandledExceptionMessage, e.Exception.Message + e.Exception.StackTrace);
            LogUtil.Error(message);
        }

        private void OnTaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            string message = string.Format(LangProvider.Instance.UnhandledExceptionMessage, e.Exception.Message + e.Exception.StackTrace);
            LogUtil.Error(message);
        }

        private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            string message = string.Format(LangProvider.Instance.UnhandledExceptionMessage, ex?.Message + ex?.StackTrace);
            LogUtil.Error(message);
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
            // new PopIcEditView().Show();
#else
            new WinMainView().Show();
#endif
        }
    }

}
