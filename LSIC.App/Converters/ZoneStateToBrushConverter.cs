namespace LSIC.App.Converters
{
    public class ZoneStateToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0] switch
            {
                ZoneState.Init or ZoneState.Idle or ZoneState.Ready or ZoneState.Stop => "PrimaryBrush".Resource<Brush>(),
                ZoneState.PowerUp or ZoneState.Burning or ZoneState.PowerDown => "SuccessBrush".Resource<Brush>(),
                ZoneState.DeviceChecking or ZoneState.DpsChecking or ZoneState.Upgrading or ZoneState.Transition => "WarningBrush".Resource<Brush>(),
                ZoneState.Offline => "SecondForegroundBrush".Resource<Brush>(),
                ZoneState.DeviceCheckFail or ZoneState.IcError or ZoneState.Error => "ErrorBrush".Resource<Brush>(),
                _ => null
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}