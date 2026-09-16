using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ATiS.Sim.Wpf
{
    public class IncludeValueToBoolConverter : IValueConverter
    {
        public bool IsInclueToTrue { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return false;
            }
            string str = value.ToString()!;
            string[] parameters = parameter.ToString()!.Split(['|'], StringSplitOptions.RemoveEmptyEntries);
            bool result = parameters.Contains(str);
            return IsInclueToTrue ? result:!result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
