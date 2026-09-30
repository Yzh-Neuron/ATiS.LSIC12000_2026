namespace LSIC.App.Converters
{
    public class ZoneGroupingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ReadOnlyObservableCollection<Zone> zones)
            {
                return null;
            }
            int length = zones.Count;
            int groupLength;
            switch (length)
            {
                case 1:
                    groupLength = 1;
                    break;
                case 16:
                case 32:
                case 64:
                    groupLength = 16;
                    break;
                case 48:
                    groupLength = 12;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            int groupCount = (int)Math.Ceiling(length * 1d / groupLength);
            List<Zone>[] groups = new List<Zone>[groupCount];
            for (int i = 0; i < groupCount; i++)
            {
                groups[i] = new List<Zone>();
            }
            for (int i = 0; i < length; i++)
            {
                groups[i / groupLength].Add(zones[i]);
            }
            return groups;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
