namespace LSIC.App.Converters
{
    internal class DutStateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DutState state = (DutState)value;
            switch (state)
            {
                case DutState.Idle:
                    return Brushes.Gray;
                case DutState.Test:
                    return Brushes.RoyalBlue;
                case DutState.Pass:
                    return Brushes.LimeGreen;
                case DutState.Fail:
                    return Brushes.Red;
                case DutState.Ignore:
                    return Brushes.LightGray;
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
