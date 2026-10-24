using Microsoft.Xaml.Behaviors.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ATiS.Sim.Wpf
{
    [TemplatePart(Name = PART_MainGrid, Type = typeof(Grid))]
    [TemplatePart(Name = PART_MorePanel, Type = typeof(Grid))]
    [TemplatePart(Name = PART_CloseButton, Type = typeof(Button))]
    public class Growl : Control
    {
        private const string PART_MainGrid = "PART_MainGrid";
        private const string PART_MorePanel = "PART_MorePanel";
        private const string PART_CloseButton = "PART_CloseButton";

        private Panel? _parentPanel;
        private Grid? _mainGrid;
        private Grid? _morePanel;
        private Button? _closeButton;
        private TranslateTransform? _renderTransform;

        /// <summary>
        /// 同一面板内常驻（不自动关闭）通知的数量上限，超出时关闭最旧的
        /// </summary>
        private const int MaxStayOpenCount = 5;
        private bool _isPanelDockLeft;
        private bool _showCloseButton = true;
        private int _waitSeconds = 2;
        private bool _stayOpen;
        private bool _closing;
        private DispatcherTimer? _autoCloseTimer;
        private int _timerTickCount;

        public Action<bool>? CallbackAfterClose { get; set; }

        #region 依赖属性
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(Growl), new PropertyMetadata(default(string)));

        public bool ShowDateTime
        {
            get => (bool)GetValue(ShowDateTimeProperty);
            set => SetValue(ShowDateTimeProperty, value);
        }

        public static readonly DependencyProperty ShowDateTimeProperty =
            DependencyProperty.Register(nameof(ShowDateTime), typeof(bool), typeof(Growl), new PropertyMetadata(false));

        public string Time
        {
            get => (string)GetValue(TimeProperty);
            private set => SetValue(TimeProperty, value);
        }

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register(nameof(Time), typeof(string), typeof(Growl), new PropertyMetadata(GetDefaultTime(DateTime.Now)));

        public MessageType Type
        {
            get => (MessageType)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(nameof(Type), typeof(MessageType), typeof(Growl), new PropertyMetadata(MessageType.Success));

        public Geometry Icon
        {
            get => (Geometry)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(Geometry), typeof(Growl), new PropertyMetadata(default(Geometry)));

        public Brush? IconBrush
        {
            get => (Brush)GetValue(IconBrushProperty);
            set => SetValue(IconBrushProperty, value);
        }

        public static readonly DependencyProperty IconBrushProperty =
            DependencyProperty.Register(nameof(IconBrush), typeof(Brush), typeof(Growl), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// 标记面板是否靠左停靠
        /// </summary>
        public static bool GetIsPanelDockLeft(DependencyObject obj) => (bool)obj.GetValue(IsPanelDockLeftProperty);
        public static void SetIsPanelDockLeft(DependencyObject obj, bool value) => obj.SetValue(IsPanelDockLeftProperty, value);

        public static readonly DependencyProperty IsPanelDockLeftProperty =
            DependencyProperty.RegisterAttached("IsPanelDockLeft", typeof(bool), typeof(Growl), new PropertyMetadata(false));

        /// <summary>
        /// 标记是否为自动创建的容器
        /// </summary>
        public static bool GetIsCreatedAutomatically(DependencyObject obj) => (bool)obj.GetValue(IsCreatedAutomaticallyProperty);
        public static void SetIsCreatedAutomatically(DependencyObject obj, bool value) => obj.SetValue(IsCreatedAutomaticallyProperty, value);

        public static readonly DependencyProperty IsCreatedAutomaticallyProperty =
            DependencyProperty.RegisterAttached("IsCreatedAutomatically", typeof(bool), typeof(Growl), new PropertyMetadata(false));
        #endregion

        private static readonly Dictionary<Window, Panel> GrowlPanels = new Dictionary<Window, Panel>();
        static Growl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Growl), new FrameworkPropertyMetadata(typeof(Growl)));
        }
        public Growl()
        {
            CommandBindings.Add(new CommandBinding(ControlCommands.Close, OnCloseButtonClick));
            CommandBindings.Add(new CommandBinding(ControlCommands.Confirm, OnConfirmButtonClick));
            CommandBindings.Add(new CommandBinding(ControlCommands.Cancel, OnCancelButtonClick));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _mainGrid = (GetTemplateChild(PART_MainGrid) as Grid)!;
            _morePanel = (GetTemplateChild(PART_MorePanel) as Grid)!;
            _closeButton = (GetTemplateChild(PART_CloseButton) as Button)!;

            if (_mainGrid == null || _morePanel == null || _closeButton == null)
            {
                throw new InvalidOperationException($"{nameof(Growl)}.{nameof(OnApplyTemplate)} Miss：{PART_MainGrid}、{PART_MorePanel}、{PART_CloseButton}");
            }

            _renderTransform = _mainGrid.RenderTransform as TranslateTransform ?? new TranslateTransform();
            _mainGrid.RenderTransform = _renderTransform;

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () =>
            {
                UpdateAnimation();
                Visibility = Visibility.Visible;
            });
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (_showCloseButton && _closeButton != null)
                _closeButton.Visibility = Visibility.Visible;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (_closeButton != null)
                _closeButton.Visibility = Visibility.Hidden;
        }
        private void OnCloseButtonClick(object sender, ExecutedRoutedEventArgs e) => Close(false);
        private void OnConfirmButtonClick(object sender, ExecutedRoutedEventArgs e) => Close(true);
        private void OnCancelButtonClick(object sender, ExecutedRoutedEventArgs e) => Close(false);

        #region 动画
        private void StartAutoCloseTimer()
        {
            if (_autoCloseTimer != null) return;

            _autoCloseTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _autoCloseTimer.Tick += (s, e) =>
            {
                if (IsMouseOver)
                {
                    _timerTickCount = 0;
                    return;
                }

                _timerTickCount++;
                if (_timerTickCount >= _waitSeconds)
                    Close(false);
            };
            _autoCloseTimer.Start();
        }
        private void UpdateAnimation()
        {
            if (_mainGrid == null) return;

            if (Type == MessageType.Question)
            {
                _morePanel!.Visibility = Visibility.Visible;
                _morePanel.IsEnabled = true;
            }

            //动画
            double startX = _isPanelDockLeft ? -ActualWidth : ActualWidth;
            _renderTransform!.X = startX;

            var slideInAnim = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseOut }
            };
            _renderTransform.BeginAnimation(TranslateTransform.XProperty, slideInAnim);

            if (!_stayOpen)
            {
                StartAutoCloseTimer();
            }
        }
        private void Close(bool dialogResult)
        {
            // 关闭动画期间可能再次触发关闭（自动定时器/用户点击/数量裁剪并发），只允许一次
            if (_closing)
            {
                return;
            }
            _closing = true;

            if (_autoCloseTimer != null)
            {
                _autoCloseTimer.Stop();
                _autoCloseTimer.Tick -= null;
                _autoCloseTimer = null;
            }

            //动画
            double endX = _isPanelDockLeft ? -ActualWidth : ActualWidth;
            var slideOutAnim = new DoubleAnimation(endX, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseOut }
            };

            slideOutAnim.Completed += (s, e) => RemoveFromPanel();
            _renderTransform!.BeginAnimation(TranslateTransform.XProperty, slideOutAnim);

            CallbackAfterClose?.Invoke(dialogResult);
        }
        private void RemoveFromPanel()
        {
            if (_parentPanel != null)
            {
                _parentPanel.Children.Remove(this);

                // 空容器销毁
                if (_parentPanel.Children.Count == 0 && GetIsCreatedAutomatically(_parentPanel))
                {
                    var window = Window.GetWindow(_parentPanel);
                    if (window != null)
                    {
                        RemovePanelForWindow(window);
                    }
                }
            }
        }
        #endregion

        #region 调用
        public static void Clear(Window? window = null)
        {
            Application.Current.Dispatcher?.Invoke(() =>
            {
                var activeWindow = window ?? GetActiveWindow();
                if (activeWindow != null)
                {
                    if (GrowlPanels.TryGetValue(activeWindow, out Panel? panel))
                    {
                        panel.Children.Clear();
                    }
                }
            });
        }
        public static void Show(GrowlInfo info)
        {
            InitGrowlInfo(ref info);
            Application.Current.Dispatcher?.Invoke(() =>
            {
                var activeWindow = GetActiveWindow();
                if (activeWindow != null)
                {
                    var panel = GetOrCreatePanelForWindow(activeWindow, info.IsPanelDockLeft);
                    if (panel != null)
                    {
                        var growl = new Growl
                        {
                            Icon = info.IconKey?.Resource<Geometry>()!,
                            IconBrush = info.IconBrushKey?.Resource<Brush>()!,
                            Message = info.Message!,

                            Time = GetDefaultTime(info.Time),
                            ShowDateTime = info.ShowDateTime,
                            CallbackAfterClose = info.CallbackAfterClose,
                            Type = info.Type,
                            HorizontalAlignment = info.IsPanelDockLeft ? HorizontalAlignment.Left : HorizontalAlignment.Right,
                            _parentPanel = panel,
                            _isPanelDockLeft = GetIsPanelDockLeft(panel),
                            _showCloseButton = info.ShowCloseButton,
                            _stayOpen = info.StayOpen,
                            _waitSeconds = Math.Max(info.WaitSeconds, 2)
                        };

                        panel.Children.Insert(0, growl);

                        // Fatal/Question 等常驻通知不自动关闭，无人值守的异常风暴场景下会无限堆积；
                        // 超出上限时关闭最旧的常驻通知，保证最新的严重告警始终可见
                        if (info.StayOpen)
                        {
                            TrimStayOpenGrowls(panel);
                        }
                    }
                }
            });
        }
        private static void TrimStayOpenGrowls(Panel panel)
        {
            // 新通知插入在 index 0，越靠后越旧；只裁剪常驻类型，自动关闭的不受影响
            List<Growl> stayOpenGrowls = panel.Children.OfType<Growl>().Where(g => g._stayOpen).ToList();
            if (stayOpenGrowls.Count <= MaxStayOpenCount)
            {
                return;
            }

            foreach (Growl oldest in stayOpenGrowls.Skip(MaxStayOpenCount))
            {
                oldest.Close(false);
            }
        }
        private static Window GetActiveWindow()
        {
            var winPtr = InteropMethods.GetActiveWindow();
            return Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.GetHandle() == winPtr)
                ?? Application.Current.MainWindow;
        }
        private static string GetDefaultTime(DateTime time)
        {
            return string.Format("({0})", time.ToString());
        }
        private static void InitGrowlInfo(ref GrowlInfo info)
        {
            switch (info.Type)
            {
                case MessageType.Info:
                    info.IconKey ??= "Geometry.Information";
                    info.IconBrushKey ??= "InfoBrush";
                    break;
                case MessageType.Warning:
                    info.IconKey ??= "Geometry.Warning";
                    info.IconBrushKey ??= "WarningBrush";
                    break;
                case MessageType.Error:
                    info.IconKey ??= "Geometry.Error";
                    info.IconBrushKey ??= "ErrorBrush";
                    break;
                case MessageType.Success:
                    info.IconKey ??= "Geometry.Success";
                    info.IconBrushKey ??= "SuccessBrush";
                    break;
                case MessageType.Fatal:
                    info.IconKey ??= "Geometry.Fatal";
                    info.IconBrushKey ??= "FatalBrush";
                    info.StayOpen = true;
                    break;
                case MessageType.Question:
                    info.IconKey ??= "Geometry.Question";
                    info.IconBrushKey ??= "QuestionBrush";
                    info.StayOpen = true;
                    info.ShowCloseButton = false;
                    break;
            }
        }

        public static void Success(string message) => Success(new GrowlInfo { Message = message });
        public static void Success(GrowlInfo info) { info.Type = MessageType.Success; Show(info); }

        public static void Info(string message) => Info(new GrowlInfo { Message = message });
        public static void Info(GrowlInfo info) { info.Type = MessageType.Info; Show(info); }

        public static void Warning(string message) => Warning(new GrowlInfo { Message = message });
        public static void Warning(GrowlInfo info) { info.Type = MessageType.Warning; Show(info); }

        public static void Error(string message) => Error(new GrowlInfo { Message = message });
        public static void Error(GrowlInfo info) { info.Type = MessageType.Error; Show(info); }

        public static void Fatal(string message) => Fatal(new GrowlInfo { Message = message });
        public static void Fatal(GrowlInfo info) { info.Type = MessageType.Fatal; Show(info); }

        public static void Question(string message) => Question(new GrowlInfo { Message = message });
        public static void Question(GrowlInfo info) { info.Type = MessageType.Question; Show(info); }
        #endregion

        #region 面板
        private static Panel? GetOrCreatePanelForWindow(Window window, bool isPanelDockLeft)
        {
            if (GrowlPanels.TryGetValue(window, out var panel))
            {
                return panel;
            }
            panel = CreatePanelForWindow(window, isPanelDockLeft);
            if (panel != null)
            {
                GrowlPanels[window] = panel;
                window.Closed += (s, e) => RemovePanelForWindow(window);
            }
            return panel;
        }
        private static Panel? CreatePanelForWindow(Window window, bool isPanelDockLeft)
        {
            var adornerLayer = window.GetChildren<AdornerDecorator>().FirstOrDefault()?.AdornerLayer;
            if (adornerLayer == null)
            {
                return null;
            }

            var stackPanel = new ReversibleStackPanel
            {
                VerticalAlignment = VerticalAlignment.Top
            };

            // 右键清空菜单
            var clearMenuItem = new MenuItem();
            var binding = new Binding(LgK.ClearText)
            {
                Source = FrameworkLanguageProvider.Instance
            };

            var clearGeo = "Geometry.Delete".Resource<Geometry>();
            clearMenuItem.Icon = new System.Windows.Shapes.Path
            {
                Data = clearGeo,
                Width = 12,
                Height = 12,
                Stretch = Stretch.Uniform,
                Fill = clearMenuItem.Foreground,
            };
            clearMenuItem.SetBinding(HeaderedItemsControl.HeaderProperty, binding);
            clearMenuItem.Click += (s, e) => Clear(window);
            stackPanel.ContextMenu = new ContextMenu { Items = { clearMenuItem } };

            SetIsPanelDockLeft(stackPanel, isPanelDockLeft);
            SetIsCreatedAutomatically(stackPanel, true);

            var scrollViewer = new ScrollViewer
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = isPanelDockLeft ? HorizontalAlignment.Left : HorizontalAlignment.Right,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                Content = stackPanel
            };

            var adorner = new AdornerContainer(adornerLayer) { Child = scrollViewer };
            adornerLayer.Add(adorner);

            return stackPanel;
        }
        private static void RemovePanelForWindow(Window window)
        {
            if (window == null || !GrowlPanels.TryGetValue(window, out var panel))
            {
                return;
            }

            panel.Children.Clear();
            panel.ContextMenu = null;

            var adornerLayer = window.GetChildren<AdornerDecorator>().FirstOrDefault()?.AdornerLayer;
            if (adornerLayer != null)
            {
                var adorner = panel.GetParent<Adorner>();
                if (adorner != null)
                {
                    adornerLayer.Remove(adorner);
                }
            }

            GrowlPanels.Remove(window);
        }
        #endregion
    }
}
