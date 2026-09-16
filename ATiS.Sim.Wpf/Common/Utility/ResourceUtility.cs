using System;
using System.Windows;

namespace ATiS.Sim.Wpf
{
    /// <summary>
    /// 资源工具类
    /// </summary>
    public static class ResourceUtility
    {
        /// <summary>
        /// 根据资源键获取资源
        /// </summary>
        public static T? Resource<T>(this string resourceKey)
        {
            object result = Application.Current.TryFindResource(resourceKey);
            if (result != null)
            {
                return (T)result;
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)resourceKey;
            }

            return default;
        }

        /// <summary>
        /// 根据资源键获取字符串资源
        /// </summary>
        public static string ResourceString(this string resourceKey)
        {
            return resourceKey.Resource<string>()!;
        }

        /// <summary>
        /// 根据资源键获取格式化字符串资源
        /// </summary>
        public static string ResourceFormat(this string resourceKey, params object[] args)
        {
            var source = resourceKey.ResourceString();
            return string.Format(source, args);
        }

        /// <summary>
        /// 根据枚举值获取资源字符串 资源键格式为 Enum.枚举类型名称.枚举值
        /// </summary>
        public static string ResourceString<T>(this T value) where T : Enum
        {
            string resourceKey = $"{nameof(Enum)}.{value.GetType().Name}.{value}";
            string result = ResourceString(resourceKey);

            return resourceKey.Equals(result) ? value.ToString() : result;
        }
    }
}
