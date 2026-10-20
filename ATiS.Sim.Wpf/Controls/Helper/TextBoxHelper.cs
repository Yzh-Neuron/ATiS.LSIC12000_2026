using System.Windows;

namespace ATiS.Sim.Wpf
{
    public class TextBoxHelper
    {
        public static readonly DependencyProperty SelectAllOnGotFocusProperty =
               DependencyProperty.RegisterAttached("SelectAllOnGotFocus", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(false, OnSelectAllOnGotFocusChanged));

        public static readonly DependencyProperty RefreshOnLostFocusProperty =
               DependencyProperty.RegisterAttached("RefreshOnLostFocus", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(false, OnRefreshOnLostFocusChanged));

        public static bool GetRefreshOnLostFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(RefreshOnLostFocusProperty);
        }

        public static void SetRefreshOnLostFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(RefreshOnLostFocusProperty, value);
        }

        public static bool GetSelectAllOnGotFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectAllOnGotFocusProperty);
        }

        public static void SetSelectAllOnGotFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectAllOnGotFocusProperty, value);
        }

        private static void OnRefreshOnLostFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is System.Windows.Controls.TextBox textBox)
            {
                if ((bool)e.NewValue)
                    textBox.LostFocus += OnLostFocus;
                else
                    textBox.LostFocus -= OnLostFocus;
            }
        }

        private static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            textBox?.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateTarget();
        }

        private static void OnSelectAllOnGotFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is System.Windows.Controls.TextBox tb)
            {
                if ((bool)e.NewValue)
                {
                    tb.GotFocus += OnGotFocus;
                }
                else
                {
                    tb.GotFocus -= OnGotFocus;
                }
            }
        }

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox tb)
            {
                tb.Dispatcher.BeginInvoke(() =>
                {
                    tb.SelectAll();

                });
            }
        }
    }
}
