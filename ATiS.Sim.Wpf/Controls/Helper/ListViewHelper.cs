using System.Windows;

namespace ATiS.Sim.Wpf
{
    public class ListViewHelper
    {
        public static readonly DependencyProperty CanUserResizeColumnsProperty =DependencyProperty.RegisterAttached(
                "CanUserResizeColumns", typeof(bool), typeof(ListViewHelper), new FrameworkPropertyMetadata(ValueBoxes.TrueBox, FrameworkPropertyMetadataOptions.Inherits));
        public static bool GetCanUserResizeColumns(DependencyObject obj)
            => (bool)obj.GetValue(CanUserResizeColumnsProperty);
        public static void SetCanUserResizeColumns(DependencyObject obj, bool value)
            => obj.SetValue(CanUserResizeColumnsProperty, ValueBoxes.BooleanBox(value));

    }
}
