namespace LSIC.App.Converters
{
    /// <summary>
    /// 水冷机状态 → 状态色（随主题刷新）
    /// </summary>
    public class WaterCoolerStateToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0] switch
            {
                WaterCoolerState.Running => "SuccessBrush".Resource<Brush>(),
                WaterCoolerState.Idle => "PrimaryBrush".Resource<Brush>(),
                WaterCoolerState.Fault => "ErrorBrush".Resource<Brush>(),
                _ => "SecondForegroundBrush".Resource<Brush>()
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
