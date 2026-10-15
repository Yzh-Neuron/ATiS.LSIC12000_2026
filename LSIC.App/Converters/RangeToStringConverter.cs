namespace LSIC.App.Converters
{
    public class RangeToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null || values.Length < 1)
            {
                return string.Empty;
            }

            if (!(values[0] is Range range))
            {
                return string.Empty;
            }

            long? vol = values.Length >= 2 ? ToLongOrNull(values[1]) : null;
            var unit = parameter as string ?? string.Empty;
            return range.ToString(vol, 1000, string.Empty, 1000, unit);
        }

        private static long? ToLongOrNull(object value)
        {
            return value switch
            {
                null => null,
                int i => i,
                long l => l,
                short s => s,
                byte b => b,
                _ => null,
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
