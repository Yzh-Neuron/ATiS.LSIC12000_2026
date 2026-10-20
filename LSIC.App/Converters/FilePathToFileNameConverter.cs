namespace LSIC.App.Converters
{
    public class FilePathToFileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is not string path || string.IsNullOrWhiteSpace(path) ? null : path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
