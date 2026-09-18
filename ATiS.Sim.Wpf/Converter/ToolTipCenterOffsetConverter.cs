using System;
using System.Globalization;
using System.Windows.Data;

namespace ATiS.Sim.Wpf
{
    /// <summary>
    /// 让 ToolTip 整体在宿主上方居中
    /// </summary>
    public class ToolTipCenterOffsetConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 &&
                values[0] is double targetWidth &&
                values[1] is double toolTipWidth &&
                targetWidth > 0 && toolTipWidth > 0)
            {
                return (targetWidth - toolTipWidth) / 2;
            }
            return 0d;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}