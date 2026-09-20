using System;
using System.Globalization;
using System.Windows.Data;

namespace ATiS.Sim.Wpf
{
    public class NullObjectToBoolConverter : IValueConverter
    {
        public bool IsNullToTrue { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNull = value == null;
            string? strParam = parameter as string;
            return IsNullToTrue ? isNull : !isNull;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
