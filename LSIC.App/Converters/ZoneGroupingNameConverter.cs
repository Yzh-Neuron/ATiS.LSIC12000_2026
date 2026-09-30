namespace LSIC.App.Converters
{
    public class ZoneGroupingNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<Zone> { Count: > 0 } zones)
            {
                if (zones.Count == 1)
                {
                    return zones.First().Id.ToString();
                }
                return $"{zones.First().Id} - {zones.Last().Id}";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
