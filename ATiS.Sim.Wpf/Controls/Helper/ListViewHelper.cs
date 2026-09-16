using System.Windows;

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

    }
}
