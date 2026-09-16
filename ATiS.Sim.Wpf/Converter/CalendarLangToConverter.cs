using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ATiS.Sim.Wpf
{
    public class CalendarLangToConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || values[0] == DependencyProperty.UnsetValue)
            {
                return values[0];
            }

            var param = parameter.ToString()!;
            var lang = (UiLanguage)values[1];

            var sr = values[0].ToString();

            if (string.IsNullOrEmpty(sr))
            {
                return values[0];
            }

            if (param.Equals("Header"))
            {
                if (DateTime.TryParse(sr, out DateTime dt))
                {
                    if (lang == UiLanguage.Chinese)
                    {
                        return dt.ToString("Y", CultureInfo.GetCultureInfo("zh-CN"));
                    }
                    else
                    {
                        return dt.ToString("Y", CultureInfo.GetCultureInfo("en-US"));
                    }
                }
            }
            else if (param.Equals("DayTitle"))
            {
                if (lang == UiLanguage.Chinese)
                {
                    switch (sr)
                    {
                        case "Mo": return "一";
                        case "Tu": return "二";
                        case "We": return "三";
                        case "Th": return "四";
                        case "Fr": return "五";
                        case "Sa": return "六";
                        case "Su": return "日";
                    }
                }
                else
                {
                    switch (sr)
                    {
                        case "一": return "Mo";
                        case "二": return "Tu";
                        case "三": return "We";
                        case "四": return "Th";
                        case "五": return "Fr";
                        case "六": return "Sa";
                        case "日": return "Su";
                    }
                }
            }
            else if (param.Equals("YearMonth"))
            {
                if (lang == UiLanguage.Chinese)
                {
                    switch (sr)
                    {
                        case "Jan": return "1月";
                        case "Feb": return "2月";
                        case "Mar": return "3月";
                        case "Apr": return "4月";
                        case "May": return "5月";
                        case "Jun": return "6月";
                        case "Jul": return "7月";
                        case "Aug": return "8月";
                        case "Sep": return "9月";
                        case "Oct": return "10月";
                        case "Nov": return "11月";
                        case "Dec": return "12月";
                    }
                }
                else
                {
                    switch (sr)
                    {
                        case "1月": return "Jan";
                        case "2月": return "Feb";
                        case "3月": return "Mar";
                        case "4月": return "Apr";
                        case "5月": return "May";
                        case "6月": return "Jun";
                        case "7月": return "Jul";
                        case "8月": return "Aug";
                        case "9月": return "Sep";
                        case "10月": return "Oct";
                        case "11月": return "Nov";
                        case "12月": return "Dec";
                    }
                }
            }

            return sr;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
