namespace LSIC.App.Converters
{
    public class DpsStateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DpsState state = (DpsState)value;
            switch (state)
            {
                case DpsState.Output:
                    return Brushes.LimeGreen;
                case DpsState.Ovp:
                case DpsState.Uvp:
                case DpsState.Ocp:
                    return Brushes.Red;
                case DpsState.Idle:
                    return Brushes.Gray;
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
