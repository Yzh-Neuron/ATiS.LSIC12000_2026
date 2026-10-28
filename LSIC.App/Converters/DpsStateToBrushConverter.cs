namespace LSIC.App.Converters
{
    public class DpsStateToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            switch (values[0])
            {
                case DpsState.Off:
                    {
                        return "SecondForegroundBrush".Resource<Brush>();
                    }
                case DpsState.Runing:
                    {
                        return "SuccessBrush".Resource<Brush>();
                    }
                default:
                    {
                        return "ErrorBrush".Resource<Brush>();
                    }
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}