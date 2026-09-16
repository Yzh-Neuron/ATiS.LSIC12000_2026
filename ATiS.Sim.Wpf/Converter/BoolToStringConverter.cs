using System;
using System.Globalization;
using System.Windows.Data;

namespace ATiS.Sim.Wpf
{
    public class BoolToStringConverter : IValueConverter
    {
        public string TrueString { get; set; } = "True";

        public string FalseString { get; set; } = "False";

        public string NullOrInvalidString { get; set; } = "";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue ? boolValue ? TrueString : FalseString : NullOrInvalidString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
