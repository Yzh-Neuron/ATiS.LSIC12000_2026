using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ATiS.Sim.Wpf
{
    /// <summary>
    /// 元素加载完成后自动获取键盘焦点
    /// </summary>
    public static class FocusHelper
    {
        public static readonly DependencyProperty FocusOnLoadProperty =
            DependencyProperty.RegisterAttached("FocusOnLoad", typeof(bool), typeof(FocusHelper), new PropertyMetadata(ValueBoxes.FalseBox, OnFocusOnLoadChanged));

        public static bool GetFocusOnLoad(DependencyObject element)
            => (bool)element.GetValue(FocusOnLoadProperty);

        public static void SetFocusOnLoad(DependencyObject element, bool value)
            => element.SetValue(FocusOnLoadProperty, ValueBoxes.BooleanBox(value));

        private static void OnFocusOnLoadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement fe)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                fe.Loaded += OnLoaded;

                if (fe.IsLoaded)
                {
                    ScheduleFocus(fe);
                }
            }
            else
            {
                fe.Loaded -= OnLoaded;
            }
        }

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe)
            {
                ScheduleFocus(fe);
            }
        }

        private static void ScheduleFocus(FrameworkElement fe)
        {
            fe.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                if (!fe.IsLoaded)
                {
                    return;
                }

                Keyboard.Focus(fe);
                if (fe is TextBox tb)
                {
                    tb.SelectAll();
                }
            }));
        }
    }
}