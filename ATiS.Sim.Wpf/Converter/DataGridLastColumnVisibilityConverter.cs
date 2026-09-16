using System;
using System.Globalization;
using System.Windows.Data;

namespace ATiS.Sim.Wpf
{
    public class DataGridLastColumnToTrueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int displayIndex)
            {
                if (displayIndex < 0)
                {
                    return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
