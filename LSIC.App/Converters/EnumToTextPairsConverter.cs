using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace LSIC.App.Converter
{
    public class EnumToTextPairsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<EnumPairItem> pairs = new List<EnumPairItem>();
            if (value is null)
            {
                return pairs;
            }
            Type type = (Type)value;
            if (!type.IsEnum)
            {
                return pairs;
            }
            Array items = Enum.GetValues(type);
            foreach (object item in items)
            {
                string text = ((Enum)item).GetString();
                pairs.Add(new EnumPairItem(item, text));
            }
            return pairs;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public struct EnumPairItem
    {
        public EnumPairItem(object key, string value)
        {
            Key = key;
            Value = value;
        }

        public object Key { get; }

        public string Value { get; }
    }
}
