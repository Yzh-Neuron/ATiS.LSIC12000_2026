namespace LSIC.App.Converters
{
    public class ZoneStateToBrushConverter : IValueConverter
    {
        private static Brush Res(object key) => (Brush)Application.Current.FindResource(key);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ZoneState)value switch
            {
                ZoneState.Init or ZoneState.Idle or ZoneState.Ready or ZoneState.Stop => Res("PrimaryBrush"),
                ZoneState.PowerUp or ZoneState.Burning or ZoneState.PowerDown => Res("SuccessBrush"),
                ZoneState.Sending or ZoneState.DeviceChecking or ZoneState.DpsChecking
                    or ZoneState.Upgrading or ZoneState.Transition => Res("WarningBrush"),
                ZoneState.Offline => Res("SecondForegroundBrush"),
                ZoneState.DeviceCheckFail or ZoneState.IcError or ZoneState.Error => Res("ErrorBrush"),
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}