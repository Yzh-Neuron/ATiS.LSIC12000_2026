namespace LSIC.App.Views
{
    public partial class CtrFlowLogView
    {
        private static readonly Brush[] LogLevelBrushes;
        private readonly BlockCollection _logsHost;
        private bool _menuIsOpen;
        private readonly int _maxCount = 500;

        #region FlowLogger 依赖属性
        public static readonly DependencyProperty LoggerProperty =
            DependencyProperty.Register(nameof(Logger), typeof(FlowLogger), typeof(CtrFlowLogView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnLoggerChanged));
        private static void OnLoggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not CtrFlowLogView view)
            {
                return;
            }

            if (e.OldValue is FlowLogger oldLogger)
            {
                oldLogger.OnAddFlow -= view.OnHistoryAppened;
                view._logsHost.Clear();
            }

            if (e.NewValue is FlowLogger newLogger)
            {
                view._logsHost.Clear();
                view.FilterItems = new ObservableCollection<FrameFilterItem>(FrameFilterUtility.GetFilterItems(newLogger));
                view.Refresh();
                newLogger.OnAddFlow -= view.OnHistoryAppened;
                newLogger.OnAddFlow += view.OnHistoryAppened;
            }
        }
        public FlowLogger Logger
        {
            get => (FlowLogger)GetValue(LoggerProperty);
            set => SetValue(LoggerProperty, value);
        }
        #endregion

        #region 过滤元素集合
        public static readonly DependencyProperty FilterItemsProperty =
            DependencyProperty.Register(nameof(FilterItems), typeof(ObservableCollection<FrameFilterItem>), typeof(CtrFlowLogView), new FrameworkPropertyMetadata(new ObservableCollection<FrameFilterItem>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ObservableCollection<FrameFilterItem> FilterItems
        {
            get => (ObservableCollection<FrameFilterItem>)GetValue(FilterItemsProperty);
            set => SetValue(FilterItemsProperty, value);
        }
        #endregion
        static CtrFlowLogView()
        {
            LogLevel[] levels = Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>().OrderBy(i => i).ToArray();
            LogLevelBrushes = new Brush[(byte)levels.Max() + 1];
            LogLevelBrushes[(int)LogLevel.Debug] = Brushes.Gray;
            LogLevelBrushes[(int)LogLevel.Info] = Brushes.Black;
            LogLevelBrushes[(int)LogLevel.Warn] = Brushes.Orange;
            LogLevelBrushes[(int)LogLevel.Error] = Brushes.Red;
            LogLevelBrushes[(int)LogLevel.Fatal] = Brushes.DarkRed;
        }
        public CtrFlowLogView()
        {
            InitializeComponent();

            _logsHost = LogView.Document.Blocks;
            _logsHost.Clear();

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Logger is not null)
            {
                Logger.OnAddFlow -= OnHistoryAppened;
                Logger.OnAddFlow += OnHistoryAppened;
            }
            Refresh();
            TryScrollToEnd();
        }
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (Logger is not null)
            {
                Logger.OnAddFlow -= OnHistoryAppened;
            }
        }
        private void OnHistoryAppened(Flow flow)
        {
            if (_logsHost.Count >= _maxCount)
            {
                _logsHost.Remove(_logsHost.FirstBlock);
            }
            _logsHost.Add(LogToParagraph(flow));
            TryScrollToEnd();
        }
        private void TryScrollToEnd()
        {
            if (!LogView.IsMouseOver && !_menuIsOpen)
            {
                LogView.ScrollToEnd();
            }
        }
        private void OnClearLogs(object sender, RoutedEventArgs e)
        {
            Logger?.History.Clear();
            Refresh();
        }
        private void Refresh()
        {
            if (_logsHost is null || Logger is null)
            {
                return;
            }

            _logsHost.Clear();

            foreach (Flow log in Logger.History)
            {
                _logsHost.Add(LogToParagraph(log));
            }

            TryScrollToEnd();
        }
        private void OnMenuVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _menuIsOpen = (bool)e.NewValue;
        }
        private static Paragraph LogToParagraph(Flow flow)
        {
            Paragraph result = new Paragraph
            {
                LineHeight = 5,
            };
            Run levelRun = new Run($"[ {flow.Time:HH:mm:ss} {flow.Level.GetString()} ]")
            {
                Foreground = LogLevelBrushes[(int)flow.Level]
            };
            Run messageRun = new Run($" {flow.Message}")
            {
                Foreground = Brushes.Black
            };
            result.Inlines.Add(levelRun);
            result.Inlines.Add(messageRun);
            return result;
        }

    }
}
