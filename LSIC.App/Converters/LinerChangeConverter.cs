namespace LSIC.App.Converters
{
    public class LinerChangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double num = System.Convert.ToDouble(value);
            string[] array = (parameter?.ToString())?.Split('|');
            if (array is not null && array.Length < 2)
            {
                return 0;
            }

            double num2 = System.Convert.ToDouble(array[0]);
            double num3 = System.Convert.ToDouble(array[1]);
            return num * num2 + num3;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
