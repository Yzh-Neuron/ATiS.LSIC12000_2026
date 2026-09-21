namespace LSIC.App.Converters
{
    public class IntegerRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = System.Convert.ToInt32(value);
            int start = parameter == null ? 0 : System.Convert.ToInt32(parameter);
            int[] arr = new int[count];
            for (int i = start; i < count; i++)
            {
                arr[i] = i;
            }
            return arr;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
