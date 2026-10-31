using System.Runtime.ExceptionServices;
using System.Windows.Controls;

namespace LSIC.App.Views
{
    public partial class CtrFlowLogView
    {
        private const int MaxCount = 600; //上限值
        private const int RemainCount = 300; //每次删除保留300条
        private const int HardLimit = 3000; //最大限制条数
        private const long UserScrollResumeDelayMs = 10000;
        private volatile bool _isActive;
        private ObservableCollection<Flow> _flows = new ObservableCollection<Flow>();
        private long _lastUserScrollTick;
        private ScrollViewer _scrollViewer;
        private ScrollViewer ScrollViewer
        {
            get
            {
                return _scrollViewer ?? (_scrollViewer = this.GetDesendentChild<ScrollViewer>());
            }
        }

        #region 依赖属性
        public FlowLogger Logger
        {
            get { return (FlowLogger)GetValue(LoggerProperty); }
            set { SetValue(LoggerProperty, value); }
        }
        public static readonly DependencyProperty LoggerProperty =
             DependencyProperty.Register(nameof(Logger), typeof(FlowLogger), typeof(CtrFlowLogView), new FrameworkPropertyMetadata(null, OnLoggerChanged));
        private static void OnLoggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CtrFlowLogView)?.UpdateLogger((FlowLogger)e.OldValue, (FlowLogger)e.NewValue);
        }
        public ObservableCollection<FrameFilterItem> FilterItems
        {
            get { return (ObservableCollection<FrameFilterItem>)GetValue(FilterItemsProperty); }
            set { SetValue(FilterItemsProperty, value); }
        }
        public static readonly DependencyProperty FilterItemsProperty =
            DependencyProperty.Register(nameof(FilterItems), typeof(ObservableCollection<FrameFilterItem>), typeof(CtrFlowLogView), new PropertyMetadata(null));
        private void UpdateLogger(FlowLogger oldLogger, FlowLogger newLogger)
        {
            Unsubscribe(oldLogger);

            if (newLogger == null)
            {
                _flows.Clear();

                return;
            }

            SetCurrentValue(FilterItemsProperty, new ObservableCollection<FrameFilterItem>(FrameFilterUtility.GetFilterItems(newLogger)));
            Refresh();
        }
        #endregion
        public CtrFlowLogView()
        {
            InitializeComponent();

            ItemsSource = _flows;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            IsVisibleChanged += OnIsVisibleChanged;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetActive(IsVisible);
        }
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            SetActive(false);
        }
        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetActive(IsLoaded && IsVisible);
        }
        private void OnHistoryAppened(Flow flow)
        {
            if (!_isActive || flow == null)
            {
                return;
            }

            Dispatcher.Invoke(() =>
            {
                _flows.Add(flow);

                bool follow = Environment.TickCount64 - _lastUserScrollTick > UserScrollResumeDelayMs;

                if (!follow && _flows.Count > HardLimit)
                {
                    _lastUserScrollTick = 0;
                    follow = true;
                }

                if (follow)
                {
                    if (_flows.Count > MaxCount)
                    {
                        int removeCount = _flows.Count - RemainCount;
                        for (int i = 0; i < removeCount; i++)
                        {
                            _flows.RemoveAt(0);
                        }
                    }

                    ScrollViewer?.ScrollToEnd();
                }

            }, DispatcherPriority.Background);
        }

        #region 核心方法
        private void SetActive(bool active)
        {
            if (_isActive != active)
            {
                _isActive = active;

                if (active)
                {
                    Refresh();
                    ScrollViewer?.ScrollToEnd();
                }
                else
                {
                    Unsubscribe(Logger);
                }
            }
        }
        private void Refresh()
        {
            if (_isActive)
            {
                LoadHistory();
                Subscribe();
            }
        }
        private void LoadHistory()
        {
            _lastUserScrollTick = 0;

            if (Logger != null)
            {
                _flows.Clear();

                foreach (var history in Logger.History)
                {
                    _flows.Add(history);
                }
            }
        }
        private void Subscribe()
        {
            if (Logger != null)
            {
                Logger.OnAddFlow -= OnHistoryAppened;
                Logger.OnAddFlow += OnHistoryAppened;
            }
        }
        private void Unsubscribe(FlowLogger logger)
        {
            if (logger != null)
            {
                logger.OnAddFlow -= OnHistoryAppened;
            }
        }
        #endregion

        #region 菜单操作
        private void OnClearLogs(object sender, RoutedEventArgs e)
        {
            Logger?.History.Clear();
            _flows.Clear();
            _lastUserScrollTick = 0;
        }
        #endregion
        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!(e.OriginalSource is ScrollViewer sv) || sv.TemplatedParent != this)
            {
                return;
            }

            if (e.ExtentHeightChange != 0)
            {
                return;
            }

            if (e.VerticalOffset + e.ViewportHeight < e.ExtentHeight - 1)
            {
                _lastUserScrollTick = Environment.TickCount64;
            }
        }
    }
}
