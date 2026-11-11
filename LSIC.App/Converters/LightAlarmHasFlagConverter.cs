namespace LSIC.App.Converters
{
    public class LightAlarmHasFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LightAlarmState state && parameter is LightAlarmState flag)
            {
                return state.HasFlag(flag);
            }

            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
