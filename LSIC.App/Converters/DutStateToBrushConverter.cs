namespace LSIC.App.Converters
{
    internal class DutStateToBrushConverter : IValueConverter
    {
        private static Brush Res(object key) => (Brush)Application.Current.FindResource(key);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (DutState)value switch
            {
                DutState.Idle => Res("SecondForegroundBrush"),
                DutState.Test => Res("InfoBrush"),
                DutState.Pass => Res("SuccessBrush"),
                DutState.Fail => Res("ErrorBrush"),
                DutState.Ignore => Res("ThirdlyForegroundBrush"),
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}