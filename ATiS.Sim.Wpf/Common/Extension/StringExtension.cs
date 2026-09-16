using System.ComponentModel;

namespace System
{
    public static class StringExtension
    {
        public static T? OfValue<T>(this string input)
        {
            try
            {
                return (T?)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(input);
            }
            catch
            {
                return default;
            }
        }

        public static object? OfValue(this string input, Type type)
        {
            try
            {
                return TypeDescriptor.GetConverter(type).ConvertFromString(input);
            }
            catch
            {
                return null;
            }
        }
    }
}
