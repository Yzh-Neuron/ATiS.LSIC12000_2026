using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace ATiS.Sim.Wpf
{
    public class ValidationErrorsToStringConverter : IMultiValueConverter
    {
        public object? Convert(object?[]? values, Type? targetType, object? parameter, CultureInfo culture)
        {
            var messages = new HashSet<string>();

            if (values is { Length: > 0 } && values[0] is IEnumerable errors)
            {
                foreach (var item in errors)
                {
                    if (item is ValidationError ve)
                    {
                        AppendContent(ve.ErrorContent, messages);
                    }
                }
            }

            if (messages.Count == 0) return null;

            string sep = parameter as string ?? Environment.NewLine;
            return string.Join(sep, messages);
        }

        private static void AppendContent(object? content, HashSet<string> messages)
        {
            switch (content)
            {
                case null:
                    break;

                case string s:
                    if (!string.IsNullOrWhiteSpace(s)) messages.Add(s);
                    break;

                case IEnumerable enumerable:
                    foreach (var o in enumerable)
                    {
                        var msg = o switch
                        {
                            System.ComponentModel.DataAnnotations.ValidationResult vr => vr.ErrorMessage,
                            _ => o?.ToString()
                        };
                        if (!string.IsNullOrWhiteSpace(msg)) messages.Add(msg!);
                    }
                    break;

                default:
                    var m = content.ToString();
                    if (!string.IsNullOrWhiteSpace(m)) messages.Add(m);
                    break;
            }
        }

        public object[] ConvertBack(object? value, Type[]? targetTypes, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}