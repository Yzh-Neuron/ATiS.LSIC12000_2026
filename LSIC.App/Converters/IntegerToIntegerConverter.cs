namespace LSIC.App.Converters
{
    public class IntegerToIntegerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }

            if (s == "-" || s == "." || s == "-." || s.EndsWith("."))
            {
                return Binding.DoNothing;
            }

            if (!int.TryParse(s, NumberStyles.Number, culture, out var val))
            {
                return null;
            }

            return val;
        }
    }
}
