namespace LSIC.App.Converters
{
    public class IntegerToDecimalConverter : IValueConverter
    {
        public decimal Scale { get; set; } = 1000;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i)
            {
                return (i / Scale);
            }

            if (value is long l)
            {
                return (l / Scale);
            }

            return null;
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

            if (!decimal.TryParse(s, NumberStyles.Number, culture, out var val))
            {
                return null;
            }

            var minVal = int.MinValue / Scale;
            var maxVal = int.MaxValue / Scale;
            if (val < minVal || val > maxVal)
            {
                return null;
            }

            var floored = decimal.Floor(decimal.Multiply(val, Scale));


            if (floored < int.MinValue || floored > int.MaxValue)
            {
                return null;
            }

            return decimal.ToInt32(floored);
        }
    }
}
