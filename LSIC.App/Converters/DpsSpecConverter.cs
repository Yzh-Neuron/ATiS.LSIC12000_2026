namespace LSIC.App.Converters
{
    internal class DpsSpecConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Dps dps)
            {
                var dpsInfo = Device.Instance.SystemConfig.DpsInfos.FirstOrDefault(t => t.Id == dps.Id);

                if (dpsInfo != null)
                {
                    string vol = FormatDecimalWithConditionalRounding(new decimal(dpsInfo.VolRange.GetCeilingValue() * 1d / 1000), 2);
                    string cur = FormatDecimalWithConditionalRounding(new decimal(dpsInfo.CurRange.GetCeilingValue() * 1d / 1000), 2);
                    return $"{vol}V/{cur}A";
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string FormatDecimalWithConditionalRounding(decimal value, int places)
        {
            if (places < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(places));
            }
            string formatted = value.ToString("F" + places);
            if (places == 0)
            {
                return formatted;
            }

            string integerPart = formatted.Split('.')[0];
            string decimalPart = formatted.Split('.')[1];

            if (decimalPart.Length > places &&
                decimalPart[places] != '0')
            {
                return formatted;
            }

            decimalPart = decimalPart.TrimEnd('0');
            if (decimalPart == "")
            {
                return integerPart;
            }

            return integerPart + "." + decimalPart;
        }
    }
}
