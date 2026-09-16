using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ATiS.Sim.Wpf
{
    public class IncludeValueToVisibilityConverter : IValueConverter
    {
        public bool IsInclueToVisible { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return IsInclueToVisible ? ValueBoxes.VisibilityBox(Visibility.Collapsed) : ValueBoxes.VisibilityBox(Visibility.Visible);
            }

            string str = value.ToString()!;
            string[] parameters = parameter.ToString()!.Split(['|'], StringSplitOptions.RemoveEmptyEntries);
            return parameters.Contains(str)
                 ? IsInclueToVisible ? ValueBoxes.VisibilityBox(Visibility.Visible) : ValueBoxes.VisibilityBox(Visibility.Collapsed)
                 : IsInclueToVisible ? ValueBoxes.VisibilityBox(Visibility.Collapsed) : ValueBoxes.VisibilityBox(Visibility.Visible);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
