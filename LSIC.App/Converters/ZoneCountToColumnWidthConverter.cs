namespace LSIC.App.Converters
{
    public class ZoneCountToColumnWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int count || parameter is not string colSide)
                return new GridLength(1, GridUnitType.Star);

            if (count == 1)
            {
                // Zones=1：左侧宽度0，右侧占满*
                return colSide == "Left"
                    ? new GridLength(0)
                    : new GridLength(1, GridUnitType.Star);
            }
            else
            {
                // 数量≠1：左1*，右2.3*
                return colSide == "Left"
                    ? new GridLength(1, GridUnitType.Star)
                    : new GridLength(2.3, GridUnitType.Star);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
