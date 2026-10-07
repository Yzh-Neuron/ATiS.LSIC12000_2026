namespace LSIC.App.Converters
{
    public class ZoneStateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ZoneState state)
            {
                return Brushes.Gray;
            }
            switch (state)
            {
                case ZoneState.Init:
                case ZoneState.Idle:
                case ZoneState.Ready:
                case ZoneState.Suspend:
                case ZoneState.Stop:
                case ZoneState.Finish:
                    return Brushes.RoyalBlue;
                case ZoneState.PowerUp:
                case ZoneState.Burning:
                case ZoneState.PowerDown:
                    return Brushes.LimeGreen;
                case ZoneState.IcSending:
                case ZoneState.SelfCheck:
                case ZoneState.Upgrading:
                    return Brushes.Orange;
                case ZoneState.Offline:
                    return Brushes.Gray;
                case ZoneState.Error:
                    return Brushes.Red;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
