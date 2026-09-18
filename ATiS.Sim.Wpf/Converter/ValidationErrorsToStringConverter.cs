using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace ATiS.Sim.Wpf
{
    public class ValidationErrorsToStringConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<ValidationError> errors)
            {
                var messages = errors
                    .Select(e => e.ErrorContent?.ToString())
                    .Where(m => !string.IsNullOrWhiteSpace(m))
                    .ToList();

                if (messages.Count == 0)
                {
                    return null;
                }

                string sep = parameter as string ?? Environment.NewLine;
                return string.Join(sep, messages);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
