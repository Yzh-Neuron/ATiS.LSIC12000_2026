namespace LSIC.App.Converters
{
    public class ZoneStateToEnableConverter : IValueConverter
    {
        public Func<ZoneState, bool> Func { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Func != null &&
                   Func.Invoke((ZoneState)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
