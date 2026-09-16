using System.Windows;

namespace ATiS.Sim.Wpf
{
    public class DropDownHelper
    {
        public static readonly DependencyProperty ConsistentWidthProperty = DependencyProperty.RegisterAttached(
            "ConsistentWidth", typeof(bool), typeof(DropDownHelper),
            new FrameworkPropertyMetadata(ValueBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetConsistentWidth(DependencyObject element, bool value)
            => element.SetValue(ConsistentWidthProperty, ValueBoxes.BooleanBox(value));

        public static bool GetConsistentWidth(DependencyObject element)
            => (bool)element.GetValue(ConsistentWidthProperty);
    }
}
