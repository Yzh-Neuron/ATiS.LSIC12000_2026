using System.Windows;

namespace ATiS.Sim.Wpf
{
    public class WidgetHelper
    {
        public static readonly DependencyProperty ShowClearButtonProperty = DependencyProperty.RegisterAttached(
            "ShowClearButton", typeof(bool), typeof(WidgetHelper),
            new FrameworkPropertyMetadata(ValueBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetShowClearButton(DependencyObject element, bool value)
            => element.SetValue(ShowClearButtonProperty, ValueBoxes.BooleanBox(value));

        public static bool GetShowClearButton(DependencyObject element)
            => (bool)element.GetValue(ShowClearButtonProperty);


        public static readonly DependencyProperty HasReadonlyIconProperty = DependencyProperty.RegisterAttached(
            "HasReadonlyIcon", typeof(bool), typeof(WidgetHelper),
            new FrameworkPropertyMetadata(ValueBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetHasReadonlyIcon(DependencyObject element, bool value)
            => element.SetValue(HasReadonlyIconProperty, ValueBoxes.BooleanBox(value));

        public static bool GetHasReadonlyIcon(DependencyObject element)
            => (bool)element.GetValue(HasReadonlyIconProperty);

        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.RegisterAttached(
            "PlaceholderText", typeof(string), typeof(WidgetHelper), new PropertyMetadata(default(string)));

        public static void SetPlaceholderText(DependencyObject element, string value)
            => element.SetValue(PlaceholderTextProperty, value);

        public static string GetPlaceholderText(DependencyObject element)
            => (string)element.GetValue(PlaceholderTextProperty);
    }
}
