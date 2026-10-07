namespace LSIC.App.Converters
{
    public class ZoneStateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ZoneState state = (ZoneState)value;
            switch (state)
            {
                case ZoneState.Init:
                case ZoneState.Idle:
                case ZoneState.Ready:
                case ZoneState.Stop:
                    return Brushes.RoyalBlue;
                case ZoneState.PowerUp:
                case ZoneState.Burning:
                case ZoneState.PowerDown:
                    return Brushes.LimeGreen;
                case ZoneState.Sending:
                case ZoneState.DeviceChecking:
                case ZoneState.DpsChecking:
                case ZoneState.Upgrading:
                case ZoneState.Transition:
                    return Brushes.Orange;
                case ZoneState.Offline:
                    return Brushes.Gray;
                case ZoneState.DeviceCheckFail:
                case ZoneState.IcError:
                case ZoneState.Error:
                    return Brushes.Red;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
