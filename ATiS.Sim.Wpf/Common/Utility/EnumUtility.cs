using System;
using System.Linq;

namespace ATiS.Sim.Wpf
{
    public static class EnumUtility
    {
        /// <summary>
        /// 获取所有的枚举元素
        /// </summary>
        public static T[] GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        }
    }
}
