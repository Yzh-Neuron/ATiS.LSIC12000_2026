namespace LSIC.App.Converters
{
    /// <summary>
    /// 多值转换器：使用[values[0]]作为本地化格式字符串，[values[1]]作为填充参数
    /// </summary>
    public class FormatTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is not { Length: 2 })
            {
                return Binding.DoNothing;
            }

            string formatKey = values[0] as string;

            if (string.IsNullOrEmpty(formatKey))
            {
                return Binding.DoNothing;
            }

            if (values[1] == DependencyProperty.UnsetValue)
            {
                return Binding.DoNothing;
            }

            string arg = values[1].ToString();
            return string.Format(culture, formatKey, arg);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
