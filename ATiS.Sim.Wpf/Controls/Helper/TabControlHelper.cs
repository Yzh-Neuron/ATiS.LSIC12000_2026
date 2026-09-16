using System.Windows;
using System.Windows.Media;

namespace ATiS.Sim.Wpf
{
    public class TabControlHelper
    {
        public static Brush GetHeaderPanelBackground(DependencyObject obj) => (Brush)obj.GetValue(HeaderPanelBackgroundProperty);

        public static void SetHeaderPanelBackground(DependencyObject obj, Brush value) => obj.SetValue(HeaderPanelBackgroundProperty, value);

        public static readonly DependencyProperty HeaderPanelBackgroundProperty = DependencyProperty.RegisterAttached(
            "HeaderPanelBackground", typeof(Brush), typeof(TabControlHelper), new PropertyMetadata(null));


        public static double GetHeaderHeight(DependencyObject obj) => (double)obj.GetValue(HeaderHeightProperty);

        public static void SetHeaderHeight(DependencyObject obj, double value) => obj.SetValue(HeaderHeightProperty, value);

        public static readonly DependencyProperty HeaderHeightProperty = DependencyProperty.RegisterAttached(
            "HeaderHeight", typeof(double), typeof(TabControlHelper), new PropertyMetadata(30.0));


        public static double GetHeaderWidth(DependencyObject obj) => (double)obj.GetValue(HeaderWidthProperty);

        public static void SetHeaderWidth(DependencyObject obj, double value) => obj.SetValue(HeaderWidthProperty, value);

        public static readonly DependencyProperty HeaderWidthProperty = DependencyProperty.RegisterAttached(
            "HeaderWidth", typeof(double), typeof(TabControlHelper), new PropertyMetadata(30.0));


        public static bool GetHasSeparator(DependencyObject obj) => (bool)obj.GetValue(HasSeparatorProperty);

        public static void SetHasSeparator(DependencyObject obj, bool value) => obj.SetValue(HasSeparatorProperty, ValueBoxes.BooleanBox(value));

        public static readonly DependencyProperty HasSeparatorProperty = DependencyProperty.RegisterAttached(
            "HasSeparator", typeof(bool), typeof(TabControlHelper), new PropertyMetadata(ValueBoxes.TrueBox));

    }
}
