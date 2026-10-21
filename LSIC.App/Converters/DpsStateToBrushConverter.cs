namespace LSIC.App.Converters
{
    public class DpsStateToBrushConverter : IValueConverter
    {
        private static Brush Res(object key) => (Brush)Application.Current.FindResource(key);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (DpsState)value switch
            {
                DpsState.Output => Res("SuccessBrush"),
                DpsState.Ovp or DpsState.Uvp or DpsState.Ocp => Res("ErrorBrush"),
                DpsState.Idle => Res("SecondForegroundBrush"),
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}