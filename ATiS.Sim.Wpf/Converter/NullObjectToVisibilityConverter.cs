using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ATiS.Sim.Wpf
{
    public class NullObjectToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// true: 对象为null时返回Visible；false: 对象不为null返回Visible（默认false）
        /// </summary>
        public bool IsNullToVisible { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNull = value == null;
            string? strParam = parameter as string;
            bool useHidden = (strParam ?? string.Empty).ToLower() == "hide";

            bool finalShow = IsNullToVisible ? isNull : !isNull;

            if (useHidden)
            {
                return finalShow
                    ? ValueBoxes.VisibilityBox(Visibility.Visible)
                    : ValueBoxes.VisibilityBox(Visibility.Hidden);
            }
            else
            {
                return finalShow
                    ? ValueBoxes.VisibilityBox(Visibility.Visible)
                    : ValueBoxes.VisibilityBox(Visibility.Collapsed);
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
