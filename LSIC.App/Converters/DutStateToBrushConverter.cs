namespace LSIC.App.Converters
{
    internal class DutStateToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0] switch
            {
                DutState.Idle => "SecondForegroundBrush".Resource<Brush>(),
                DutState.Test => "PrimaryBrush".Resource<Brush>(),
                DutState.Pass => "SuccessBrush".Resource<Brush>(),
                DutState.Fail => "ErrorBrush".Resource<Brush>(),
                DutState.Ignore => "IgnoreBrush".Resource<Brush>(),
                _ => null
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}