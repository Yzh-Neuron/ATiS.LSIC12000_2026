namespace LSIC.App.Converters
{
    public class ZoneCanGotoBurnConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //return values.Length == 3 &&
            //       values[0] is Zone &&
            //       values[1] is ZoneState state &&
            //       state.CanSendIc() &&
            //       values[2] is IcFile;

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
