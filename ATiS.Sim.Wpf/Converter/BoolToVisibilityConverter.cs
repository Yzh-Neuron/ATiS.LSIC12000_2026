using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ATiS.Sim.Wpf
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool IsTrueToVisible { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? val = value as bool?;
            string? strParam = parameter as string;
            bool isHide = (strParam ?? string.Empty).ToLower() == "hide";
            return isHide
                ? val.HasValue && val.Value
                    ? IsTrueToVisible ? ValueBoxes.VisibilityBox(Visibility.Visible) : ValueBoxes.VisibilityBox(Visibility.Hidden) 
                    : IsTrueToVisible ? ValueBoxes.VisibilityBox(Visibility.Hidden) : ValueBoxes.VisibilityBox(Visibility.Visible) 
                : val.HasValue && val.Value
                    ? IsTrueToVisible ? ValueBoxes.VisibilityBox(Visibility.Visible) : ValueBoxes.VisibilityBox(Visibility.Collapsed)
                    :IsTrueToVisible ? ValueBoxes.VisibilityBox(Visibility.Collapsed) : ValueBoxes.VisibilityBox(Visibility.Visible);
        }
  
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility val)
            {
                return val == Visibility.Visible;
            }

            throw new NotImplementedException();
        }
    }
}
