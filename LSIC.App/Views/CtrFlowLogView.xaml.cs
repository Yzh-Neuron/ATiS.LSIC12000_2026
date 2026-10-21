namespace LSIC.App.Views
{
    public partial class CtrFlowLogView
    {
        private readonly BlockCollection _logsHost;
        private bool _menuIsOpen;
        private readonly int _maxCount = 500;

        #region 画刷
        public Brush DebugBrush
        {
            get { return (Brush)GetValue(DebugBrushProperty); }
            set { SetValue(DebugBrushProperty, value); }
        }
        public static readonly DependencyProperty DebugBrushProperty =
            DependencyProperty.Register(nameof(DebugBrush), typeof(Brush), typeof(CtrFlowLogView), new PropertyMetadata(Brushes.Gray));
        public Brush InfoBrush
        {
            get { return (Brush)GetValue(InfoBrushProperty); }
            set { SetValue(InfoBrushProperty, value); }
        }
        public static readonly DependencyProperty InfoBrushProperty =
            DependencyProperty.Register(nameof(InfoBrush), typeof(Brush), typeof(CtrFlowLogView), new PropertyMetadata(Brushes.Black));
        public Brush WarnBrush
        {
            get { return (Brush)GetValue(WarnBrushProperty); }
            set { SetValue(WarnBrushProperty, value); }
        }
        public static readonly DependencyProperty WarnBrushProperty =
            DependencyProperty.Register(nameof(WarnBrush), typeof(Brush), typeof(CtrFlowLogView), new PropertyMetadata(Brushes.Orange));
        public Brush ErrorBrush
        {
            get { return (Brush)GetValue(ErrorBrushProperty); }
            set { SetValue(ErrorBrushProperty, value); }
        }
        public static readonly DependencyProperty ErrorBrushProperty =
            DependencyProperty.Register(nameof(ErrorBrush), typeof(Brush), typeof(CtrFlowLogView), new PropertyMetadata(Brushes.Red));
        public Brush FatalBrush
        {
            get { return (Brush)GetValue(FatalBrushProperty); }
            set { SetValue(FatalBrushProperty, value); }
        }
        public static readonly DependencyProperty FatalBrushProperty =
            DependencyProperty.Register(nameof(FatalBrush), typeof(Brush), typeof(CtrFlowLogView), new PropertyMetadata(Brushes.DarkRed));
        #endregion

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
            if (!CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<Flow>(OnHistoryAppened), flow);
                return;
            }

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
        private Paragraph LogToParagraph(Flow flow)
        {
            Paragraph result = new Paragraph
            {
                LineHeight = 5,
            };
            Run levelRun = new Run($"[ {flow.Time:HH:mm:ss} {flow.Level.GetString()} ]")
            {
                Foreground = GetBrush(flow.Level)
            };
            Run messageRun = new Run($" {flow.Message}")
            {
                Foreground = Foreground
            };
            result.Inlines.Add(levelRun);
            result.Inlines.Add(messageRun);
            return result;
        }
        private Brush GetBrush(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Debug => DebugBrush,
                LogLevel.Info => InfoBrush,
                LogLevel.Warn => WarnBrush,
                LogLevel.Error => ErrorBrush,
                LogLevel.Fatal => FatalBrush,
                _ => Foreground
            };
        }
    }
}
