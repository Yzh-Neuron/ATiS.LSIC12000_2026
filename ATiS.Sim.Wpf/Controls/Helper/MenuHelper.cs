using System.Windows;

namespace ATiS.Sim.Wpf
{
    public class MenuHelper
    {
        public static readonly DependencyProperty PopupMaxHeightProperty = DependencyProperty.RegisterAttached("PopupMaxHeight", typeof(double), typeof(MenuHelper), new FrameworkPropertyMetadata(double.MaxValue, FrameworkPropertyMetadataOptions.Inherits));

       
        public static double GetPopupMaxHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(PopupMaxHeightProperty);
        }

        public static void SetPopupMaxHeight(DependencyObject obj, double value)
        {
            obj.SetValue(PopupMaxHeightProperty, value);
        }
    }
}
