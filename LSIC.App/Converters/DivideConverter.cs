
namespace LSIC.App.Converters
{
    public class DivideConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return value;
            }

            if (!double.TryParse(value.ToString(), NumberStyles.Any, culture, out double dividend))
            {
                return value;
            }

            if (!double.TryParse(parameter.ToString(), NumberStyles.Any, culture, out double divisor))
            {
                return value;
            }

            if (divisor == 0)
            {
                return value;
            }
            return dividend / divisor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return value;
            }

            if (!double.TryParse(value.ToString(), NumberStyles.Any, culture, out double quotient))
            {
                return value;
            }

            if (!double.TryParse(parameter.ToString(), NumberStyles.Any, culture, out double divisor))
            {
                return value;
            }

            return quotient * divisor;
        }
    }
}
