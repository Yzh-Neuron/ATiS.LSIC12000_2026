using System;
using System.Globalization;
using System.Windows.Data;

namespace ATiS.Sim.Wpf
{
    public class RatioToPercentMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
            {
                return .0;
            }

            var obj1 = values[0];
            var obj2 = values[1];

            if (obj1 == null || obj2 == null)
            {
                return .0;
            }

            var str1 = values[0].ToString();
            var str2 = values[1].ToString();

            if (double.TryParse(str1,out double v1)&& double.TryParse(str2, out double v2))
            {
                if (Math.Abs(v2) < 1E-06)
                {
                    return 100.0;
                }

                return v1 / v2 * 100;
            }
            else
            {
                return .0;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}
