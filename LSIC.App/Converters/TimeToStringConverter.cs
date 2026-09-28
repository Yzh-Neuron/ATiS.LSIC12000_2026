namespace LSIC.App.Converters
{
    public class TimeToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is { Length: 2 }
                && values[0] is int value
                && values[1] is TimeUnit unit)
            {
                return $"{value} {unit.GetString()}";
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}