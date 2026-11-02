using System;
using System.Collections.Generic;
namespace LSIC.App.Converters
{
    public class IntegerToVersionConverter : IValueConverter
    {
        public bool Reverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            byte[] bytes = BitConverter.GetBytes(val);
            if (Reverse)
            {
                bytes = bytes.Reverse().ToArray();
            }
            return string.Join(".", bytes);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
