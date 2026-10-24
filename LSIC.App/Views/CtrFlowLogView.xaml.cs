using System.Windows.Controls;

namespace LSIC.App.Views
{
    public partial class CtrFlowLogView
    {
        private static readonly TimeSpan UserScrollResumeDelay = TimeSpan.FromSeconds(30);
        private const long ProgramScrollGuardMs = 100;
        private bool _atBottom = true;
        private long _lastUserScrollTick;
        private long _lastProgramScrollTick;
        private const int MaxCount = 100;
        private const double AutoScrollThreshold = 2.0;
        private ObservableCollection<Flow> _flows = new ObservableCollection<Flow>();
        private volatile bool _isActive;
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
            DependencyProperty.Register(nameof(FilterItems), typeof(ObservableCollection<FrameFilterItem>), typeof(CtrFlowLogView), new PropertyMetadata(null, OnFilterItemsChanged));
        private static void OnFilterItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrFlowLogView view)
            {
                view.Refresh();
            }
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
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _scrollViewer = null;
        }
        private void UpdateLogger(FlowLogger oldLogger, FlowLogger newLogger)
        {
            Unsubscribe(oldLogger);

            if (newLogger == null)
            {
                _flows.Clear();

                return;
            }

            SetCurrentValue(FilterItemsProperty, new ObservableCollection<FrameFilterItem>(FrameFilterUtility.GetFilterItems(newLogger)));
        }
        private void OnHistoryAppened(Flow flow)
        {
            if (!_isActive || flow == null)
            {
                return;
            }

            this.InvokeOnUI(Append, flow, DispatcherPriority.Background);
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
        private void Append(Flow flow)
        {
            //不在底部且用户超过时限未滚动，回到底部
            if (!_atBottom && Environment.TickCount64 - _lastUserScrollTick >= UserScrollResumeDelay.TotalMilliseconds)
            {
                _atBottom = true;
                ScrollToEnd();
            }

            _flows.Add(flow);

            int removed = 0;
            while (_flows.Count > MaxCount)
            {
                _flows.RemoveAt(0);
                removed++;
            }

            //有移除说明满了：不在底部时回退偏移锁定可视内容
            if (removed > 0 && !_atBottom && ScrollViewer != null)
            {
                //记为程序滚动，其回声事件不算用户活动
                _lastProgramScrollTick = Environment.TickCount64;
                ScrollViewer.ScrollToVerticalOffset(Math.Max(0, ScrollViewer.VerticalOffset - removed));
            }
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
                ScrollToEnd();
            }
        }
        private void LoadHistory()
        {
            if (Logger != null)
            {
                _flows = new ObservableCollection<Flow>(Logger.History);

                SetCurrentValue(ItemsSourceProperty, _flows);
                _atBottom = true;
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

        #region 自动跟随滚动
        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange == 0)
            {
                bool atBottom = e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight - AutoScrollThreshold;

                if (atBottom)
                {
                    _atBottom = true;
                }
                else
                {
                    if (Environment.TickCount64 - _lastProgramScrollTick > ProgramScrollGuardMs)
                    {
                        _lastUserScrollTick = Environment.TickCount64;
                    }
                    _atBottom = false;
                }
            }
            else if (_atBottom)
            {
                ScrollToEnd();
            }
        }
        private void ScrollToEnd()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_atBottom)
                {
                    _lastProgramScrollTick = Environment.TickCount64;
                    ScrollViewer?.ScrollToEnd();
                }
            }));
        }
        #endregion

        #region 菜单操作
        private void OnClearLogs(object sender, RoutedEventArgs e)
        {
            Logger?.History.Clear();
            _flows.Clear();
            _atBottom = true;
        }
        #endregion
    }
}
