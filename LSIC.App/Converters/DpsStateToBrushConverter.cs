namespace LSIC.App.Converters
{
    public class DpsStateToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0] switch
            {
                DpsState.Output => "SuccessBrush".Resource<Brush>(),
                DpsState.Ovp or DpsState.Uvp or DpsState.Ocp => "ErrorBrush".Resource<Brush>(),
                DpsState.Idle => "SecondForegroundBrush".Resource<Brush>(),
                _ => null
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}