using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ATiS.Sim.Wpf
{
    /// <summary>
    /// 把鼠标滚轮转发给父级 ScrollViewer。
    /// ListBox / DataGrid / 内层 ScrollViewer （滚动条 Disabled），
    /// </summary>
    public static class MouseWheelBubble
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(MouseWheelBubble), new PropertyMetadata(ValueBoxes.FalseBox, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject element)
            => (bool)element.GetValue(IsEnabledProperty);

        public static void SetIsEnabled(DependencyObject element, bool value)
            => element.SetValue(IsEnabledProperty, ValueBoxes.BooleanBox(value));

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement fe)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                fe.PreviewMouseWheel += OnPreviewMouseWheel;
            }
            else
            {
                fe.PreviewMouseWheel -= OnPreviewMouseWheel;
            }
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is not FrameworkElement fe)
            {
                return;
            }

            if (fe is ScrollViewer inner)
            {
                if (e.Delta > 0 && inner.VerticalOffset > 0)
                {
                    return;
                }
                if (e.Delta < 0 && inner.VerticalOffset < inner.ScrollableHeight)
                {
                    return;
                }
            }

            ScrollViewer? parent = fe.GetParent<ScrollViewer>();
            if (parent == null)
            {
                return;
            }

            parent.ScrollToVerticalOffset(parent.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}