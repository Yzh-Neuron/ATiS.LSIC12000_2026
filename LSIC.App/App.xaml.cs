using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace LSIC.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string ConfigFile = "Config.ini";
        private static Mutex _mutex;
        private static bool _createdNew;
        private static Task _checkDongleTask;
        private static bool _checkDongleStart;
        private static CancellationTokenSource _checkDongleCts;
        public App()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
            Exit += OnExit;
        }

        #region 异常捕获
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            string message = string.Format(LangProvider.Instance.UnhandledExceptionMessage, e.Exception.Message + e.Exception.StackTrace);
            SystemLog.Error(message);
        }
        private void OnTaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            string message = string.Format(LangProvider.Instance.UnhandledExceptionMessage, e.Exception.Message + e.Exception.StackTrace);
            SystemLog.Error(message);
        }
        private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            string message = string.Format(LangProvider.Instance.UnhandledExceptionMessage, ex?.Message + ex?.StackTrace);
            SystemLog.Error(message);

            // 进程即将终止：排空系统日志，保证致命异常落盘
            if (e.IsTerminating)
            {
                SystemLog.Dispose();
            }
        }
        #endregion

        #region 加密狗
        public void StartCheckDongle()
        {
            if (_checkDongleStart)
            {
                return;
            }
            _checkDongleStart = true;
            _checkDongleCts = new CancellationTokenSource();
            _checkDongleTask = Task.Factory.StartNew(async () =>
            {
                while (_checkDongleStart)
                {
                    CheckDongle(null);
                    await Task.Delay(5000, _checkDongleCts.Token);
                }
            }, TaskCreationOptions.LongRunning);
        }
        private void CheckDongle(object _)
        {
#if DEBUG
            return;
#else
            bool success = DongleHelper.Verify(out DateTime? now, out DateTime? deadline);
            if (!success)
            {
                string machine = DongleHelper.MachineCode;
                Current.Dispatcher.Invoke(() =>
                {
                    if (now.HasValue && deadline.HasValue && deadline.Value < now.Value)
                    {
                        PopIllegalDongleView window = new PopIllegalDongleView(machine, deadline);
                        if (Current.MainWindow is { IsVisible: true })
                        {
                            window.Owner = Current.MainWindow;
                        }
                        window.ShowDialog();
                    }
                    else
                    {
                        PopIllegalDongleView window = new PopIllegalDongleView(machine);
                        if (Current.MainWindow is { IsVisible: true })
                        {
                            window.Owner = Current.MainWindow;
                        }
                        window.ShowDialog();
                    }

                    Shutdown();
                });
            }
#endif
        }
        private static void StopCheckDongle()
        {
            _checkDongleStart = false;
            _checkDongleCts?.Cancel();
            try
            {
                _checkDongleTask?.Wait(1000);
            }
            catch
            {
           
            }
            _checkDongleCts?.Dispose();
        }
        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            //注册语言模块
            GlobalLanguageManager.RegisterLanguageResources(UiLanguage.Chinese, [
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/zh-CN/Message.xaml")},
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/zh-CN/UI.xaml")},
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/zh-CN/Enum.xaml")},
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/zh-CN/DeviceCheck.xaml")}]);

            GlobalLanguageManager.RegisterLanguageResources(UiLanguage.English, [
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/en-US/Message.xaml")},
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/en-US/UI.xaml")},
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/en-US/Enum.xaml")},
            new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Lang/en-US/DeviceCheck.xaml")}]);

            string appId = AssemblyUtility.GetGuid();
            _mutex = new Mutex(true, appId, out _createdNew);
            if (!_createdNew)
            {
                SystemLog.Error(LangProvider.Instance.AppHasLaunchedMessage);
                MessagePop.Show(LangProvider.Instance.AppHasLaunchedMessage, MessageType.Error);
                Shutdown();
                return;
            }

            base.OnStartup(e);
            SystemLog.Info(LangProvider.Instance.AppStartMessage);
            CheckIniFile(ConfigFile);

            // 解析配置文件
            try
            {
                SystemConfig config = new SystemConfig();

                bool successConfig = Ini.TryParse(ConfigFile, Encoding.UTF8, ref config, out int errorLineConfig);
                if (!successConfig)
                {
                    throw new Exception(errorLineConfig.ToString());
                }
                Device.Initialize(config);
            }
            catch (Exception ex)
            {
                string error = string.Format(LangProvider.Instance.DeviceConfigParseFailureMessage, ex.Message);
                SystemLog.Error(error);
                MessagePop.Show(error, MessageType.Error);
                Shutdown();
                return;
            }

            SleepUtility.PreventForCurrentThread();
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 60 });
            ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata { DefaultValue = 250 });
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata { DefaultValue = 6000 });

            PopHelper.Init();

            CheckDongle(null);

            new WinStartupView().Show();
        }
        private void CheckIniFile(string path)
        {
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
            {
                FileInfo defaultFile = new FileInfo(Path.Combine("DefaultConfig", path));
                if (defaultFile.Exists)
                {
                    defaultFile.CopyTo(file.FullName);
                }
            }
            file.Refresh();
            if (file.Exists)
            {
                return;
            }
            string error = $"{LangProvider.Instance.FileNotExistMessage}: {file.FullName}";
            SystemLog.Error(error);
            MessagePop.Show(error, messageType: MessageType.Error);
            Shutdown();
        }

        #region 退出释放
        private void OnExit(object sender, ExitEventArgs e)
        {
            StartExitWatchdog(TimeSpan.FromSeconds(10));
            StopCheckDongle();
            Device.Instance?.Dispose();
            PopHelper.Dispose();
            JobUtility.Instance.Dispose();
            SleepUtility.RestoreForCurrentThread();

            if (_createdNew)
            {
                _mutex?.ReleaseMutex();
                _mutex?.Close();
            }

            SystemLog.Info(LangProvider.Instance.AppExitMessage);
            SystemLog.Dispose();
            Environment.Exit(0);
        }
        private static void StartExitWatchdog(TimeSpan timeout)
        {
            var thread = new Thread(() =>
            {
                Thread.Sleep(timeout);
                SystemLog.Error(LangProvider.Instance.ForceExitOnTimeoutMessage);
                SystemLog.Dispose();
                Environment.Exit(1);
            })
            {
                IsBackground = true,
                Name = "ExitWatchdog"
            };
            thread.Start();
        }
        #endregion
    }
}