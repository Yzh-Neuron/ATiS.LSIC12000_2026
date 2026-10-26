using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;

namespace ATiS.Sim.Wpf
{
    public class ChromeWindow : Window
    {
        private const double ResizeGripWidth = 2.0d;
        public object TitleBarContent
        {
            get { return (object)GetValue(TitleBarContentProperty); }
            set { SetValue(TitleBarContentProperty, value); }
        }
        public static readonly DependencyProperty TitleBarContentProperty =
            DependencyProperty.Register(nameof(TitleBarContent), typeof(object), typeof(ChromeWindow), new PropertyMetadata(null));
        public bool IsShowTitleBar
        {
            get { return (bool)GetValue(IsShowTitleBarProperty); }
            set { SetValue(IsShowTitleBarProperty, value); }
        }
        public static readonly DependencyProperty IsShowTitleBarProperty =
            DependencyProperty.Register(nameof(IsShowTitleBar), typeof(bool), typeof(ChromeWindow), new PropertyMetadata(true));
        public double TitleBarHeight
        {
            get { return (double)GetValue(TitleBarHeightProperty); }
            set { SetValue(TitleBarHeightProperty, value); }
        }
        public static readonly DependencyProperty TitleBarHeightProperty =
            DependencyProperty.Register(nameof(TitleBarHeight), typeof(double), typeof(ChromeWindow), new PropertyMetadata(30.0, OnChromeLayoutPropertyChanged));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(ChromeWindow), new PropertyMetadata(new CornerRadius(0)));
        public Brush TitleBarBackground
        {
            get { return (Brush)GetValue(TitleBarBackgroundProperty); }
            set { SetValue(TitleBarBackgroundProperty, value); }
        }
        public static readonly DependencyProperty TitleBarBackgroundProperty =
            DependencyProperty.Register(nameof(TitleBarBackground), typeof(Brush), typeof(ChromeWindow), new PropertyMetadata(null));
        public Brush TitleBarForeground
        {
            get { return (Brush)GetValue(TitleBarForegroundProperty); }
            set { SetValue(TitleBarForegroundProperty, value); }
        }
        public static readonly DependencyProperty TitleBarForegroundProperty =
            DependencyProperty.Register(nameof(TitleBarForeground), typeof(Brush), typeof(ChromeWindow), new PropertyMetadata(null));
        public new Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }
        public static new readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(ChromeWindow), new PropertyMetadata(null));
        public Thickness ShadowMargin
        {
            get { return (Thickness)GetValue(ShadowMarginProperty); }
            set { SetValue(ShadowMarginProperty, value); }
        }
        public static readonly DependencyProperty ShadowMarginProperty =
            DependencyProperty.Register(nameof(ShadowMargin), typeof(Thickness), typeof(ChromeWindow), new PropertyMetadata(new Thickness(20), OnChromeLayoutPropertyChanged));
        private static void OnChromeLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChromeWindow window)
            {
                window.UpdateWindowChrome();
            }
        }

        static ChromeWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ChromeWindow), new FrameworkPropertyMetadata(typeof(ChromeWindow)));
        }

        public ChromeWindow()
        {
            AllowsTransparency = true;
            WindowStyle = WindowStyle.None;

            WindowChrome windowChrome = new WindowChrome
            {
                CaptionHeight = 0,
                CornerRadius = new CornerRadius(0),
                GlassFrameThickness = new Thickness(-1),
                UseAeroCaptionButtons = false,
                NonClientFrameEdges = NonClientFrameEdges.None
            };
            WindowChrome.SetWindowChrome(this, windowChrome);

            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            UpdateWindowChrome();
        }
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            UpdateWindowChrome();
        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == BorderThicknessProperty)
            {
                UpdateWindowChrome();
            }
        }
        private void UpdateWindowChrome()
        {
            var chrome = WindowChrome.GetWindowChrome(this);
            if (chrome != null)
            {
                if (WindowState == WindowState.Maximized)
                {
                    chrome.ResizeBorderThickness = new Thickness(0);
                    chrome.CaptionHeight = TitleBarHeight;
                }
                else
                {
                    chrome.ResizeBorderThickness = new Thickness(
                        ShadowMargin.Left + BorderThickness.Left + ResizeGripWidth,
                        ShadowMargin.Top + BorderThickness.Top + ResizeGripWidth,
                        ShadowMargin.Right + BorderThickness.Right + ResizeGripWidth,
                        ShadowMargin.Bottom + BorderThickness.Bottom + ResizeGripWidth);

                    chrome.CaptionHeight = TitleBarHeight - ResizeGripWidth;
                }
            }
        }
        private void OnMinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void OnMaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }
        private void OnRestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }
        private void OnCloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
