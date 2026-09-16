using System;
using System.Collections.Generic;
using System.Linq;

namespace ATiS.Sim.Wpf
{
    /// <summary>
    /// 类型工具类
    /// </summary>
    public static class TypeUtility
    {
        /// <summary>
        /// 给定抽象父类，获取所有继承类
        /// </summary>
        public static IEnumerable<Type> GetSubTypes(this Type baseType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(baseType));
        }

        public static object? CreateInstance(this Type type)
        {
            return Activator.CreateInstance(type);
        }

        public static T? CreateInstance<T>(this Type type)
        {
            return (T?)Activator.CreateInstance(type);
        }
    }
}
