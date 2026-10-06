namespace LSIC.App.Converters
{
    public class ProportionalWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double totalWidth = (double)value;
            if (double.IsNaN(totalWidth) || totalWidth <= 0)
            {
                return 80.0;
            }

            string[] parts = (parameter as string)?.Split(',')?? ["1", "1", "1"];
            int[] ratios = parts.Select(p => int.TryParse(p.Trim(), out int r) ? r : 1).ToArray();
            int totalRatio = ratios.Sum();

            string[] paramParts = (parameter as string)?.Split(',') ?? ["1", "1"];
            if (paramParts.Length == 2 &&
                int.TryParse(paramParts[0], out int weight) &&
                int.TryParse(paramParts[1], out int totalWeight))
            {
                return (totalWidth * weight) / totalWeight;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
