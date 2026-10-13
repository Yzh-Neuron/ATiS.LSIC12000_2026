using System;
using System.Globalization;
using System.Windows.Data;
namespace ATiS.Sim.Wpf
{
    internal class StringEmptyToBoolConverter : IValueConverter
    {
        public bool IsEmptyToTrue { get; set; } = true;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return string.IsNullOrEmpty(str) == IsEmptyToTrue;
            }

            return IsEmptyToTrue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
