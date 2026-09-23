using System.Collections;

namespace LSIC.App.Converters
{
    public class EnumerableToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not IEnumerable source)
            {
                return string.Empty;
            }

            if (parameter is not string path || string.IsNullOrWhiteSpace(path))
            {
                var list = new List<string?>();
                foreach (var item in source)
                {
                    list.Add(item?.ToString());
                }
                return string.Join(", ", list);
            }

            var names = path.Split('.');
            var result = new List<string?>();
            foreach (var item in source)
            {
                object obj = item;
                foreach (var name in names)
                {
                    obj = obj?.GetType().GetProperty(name)?.GetValue(obj);
                }
                result.Add(obj?.ToString());
            }
            return string.Join(", ", result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
