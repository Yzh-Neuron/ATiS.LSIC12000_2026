using System.Windows.Controls;

namespace LSIC.App.Views
{
    public partial class CtrFlowLogView
    {
        private const int MaxCount = 500;
        private const double AutoScrollThreshold = 2.0;
        private ObservableCollection<Flow> _flows = new ObservableCollection<Flow>();
        private volatile bool _isActive;
        private bool _autoScroll = true;
        private bool _menuIsOpen;
        private int _scrollScheduled;
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
            _flows.Add(flow);

            while (_flows.Count > MaxCount)
            {
                _flows.RemoveAt(0);
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
                RequestScrollToEnd();
            }
        }
        private void LoadHistory()
        {
            if (Logger != null)
            {
                _flows = new ObservableCollection<Flow>(Logger.History);

                SetCurrentValue(ItemsSourceProperty, _flows);
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

        #region 界面滚动
        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0)
            {
                RequestScrollToEnd();

                return;
            }

            _autoScroll = e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight - AutoScrollThreshold;
        }
        private void RequestScrollToEnd()
        {
            if (!_autoScroll || _menuIsOpen)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref _scrollScheduled, 1, 0) == 1)
            {
                return;
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                Volatile.Write(ref _scrollScheduled, 0);

                if (_autoScroll && !_menuIsOpen)
                {
                    ScrollViewer?.ScrollToEnd();
                }
            }), DispatcherPriority.Loaded);
        }
        #endregion

        #region 菜单操作
        private void OnClearLogs(object sender, RoutedEventArgs e)
        {
            Logger?.History.Clear();

            _flows.Clear();

            _autoScroll = true;
        }
        private void OnMenuVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _menuIsOpen = (bool)e.NewValue;
        }
        #endregion
    }


}
