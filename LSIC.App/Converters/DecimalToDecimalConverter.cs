namespace LSIC.App.Converters
{
    public class DecimalToDecimalConverter : IValueConverter
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

            if (!double.TryParse(s, NumberStyles.Float, culture, out var d))
            {
                return null;
            }

            return d;
        }
    }
}
