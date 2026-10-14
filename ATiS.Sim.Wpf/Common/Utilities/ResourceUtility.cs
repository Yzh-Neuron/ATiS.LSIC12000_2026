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
    }
}
