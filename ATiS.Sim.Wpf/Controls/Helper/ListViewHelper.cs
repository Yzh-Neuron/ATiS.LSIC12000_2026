using System.Windows;
using System.Windows.Media;

namespace ATiS.Sim.Wpf
{
    public class ListViewHelper
    {
        public static readonly DependencyProperty ColumnHeaderHeightProperty = DependencyProperty.RegisterAttached(
               "ColumnHeaderHeight", typeof(double), typeof(ListViewHelper), new PropertyMetadata(ValueBoxes.Double0Box));
        public static void SetColumnHeaderHeight(DependencyObject element, double value)
            => element.SetValue(ColumnHeaderHeightProperty, value);
        public static double GetColumnHeaderHeight(DependencyObject element)
            => (double)element.GetValue(ColumnHeaderHeightProperty);


        public static readonly DependencyProperty ListViewItemMinHeightProperty = DependencyProperty.RegisterAttached(
                 "ListViewItemMinHeight", typeof(double), typeof(ListViewHelper), new PropertyMetadata(ValueBoxes.Double20Box));
        public static void SetListViewItemMinHeight(DependencyObject element, double value)
            => element.SetValue(ListViewItemMinHeightProperty, value);
        public static double GetListViewItemMinHeight(DependencyObject element)
            => (double)element.GetValue(ListViewItemMinHeightProperty);


        public static readonly DependencyProperty CanUserResizeColumnsProperty =DependencyProperty.RegisterAttached(
                "CanUserResizeColumns", typeof(bool), typeof(ListViewHelper), new FrameworkPropertyMetadata(ValueBoxes.TrueBox, FrameworkPropertyMetadataOptions.Inherits));
        public static bool GetCanUserResizeColumns(DependencyObject obj)
            => (bool)obj.GetValue(CanUserResizeColumnsProperty);
        public static void SetCanUserResizeColumns(DependencyObject obj, bool value)
            => obj.SetValue(CanUserResizeColumnsProperty, ValueBoxes.BooleanBox(value));

    }
}
