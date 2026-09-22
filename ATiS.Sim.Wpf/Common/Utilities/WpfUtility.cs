using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ATiS.Sim.Wpf
{
    /// <summary>
    /// WPF 工具类
    /// </summary>
    public static class WpfUtility
    {
        /// <summary>
        /// 获取控件的模板
        /// </summary>
        public static T? FindTemplatePart<T>(this Control templatedParent, string name) where T : DependencyObject
        {
            return templatedParent.Template.FindName(name, templatedParent) as T;
        }

        /// <summary>
        /// 根据点击位置获取子元素
        /// </summary>
        public static DependencyObject? GetElement(this UIElement source, Point position)
        {
            DependencyObject? element = VisualTreeHelper.HitTest(source, position)?.VisualHit;
            return element;
        }

        /// <summary>
        /// 根据泛型类型获取子元素
        /// </summary>
        public static IEnumerable<T> GetChildren<T>(this DependencyObject source) where T : DependencyObject
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is T result)
            {
                yield return result;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(source); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(source, i);
                foreach (T deresult in child.GetChildren<T>())
                {
                    yield return deresult;
                }
            }
        }

        /// <summary>
        /// 获取指定类型的后代子元素，找到第一个就返回
        /// </summary>
        public static T? GetDesendentChild<T>(this DependencyObject target) where T : DependencyObject
        {
            int count = VisualTreeHelper.GetChildrenCount(target);
            if (count == 0)
            {
                return null;
            }
            for (int i = 0; i < count; i++)
            {
                DependencyObject current = VisualTreeHelper.GetChild(target, i);
                if (current is T t)
                {
                    return t;
                }
                T? desendent = current.GetDesendentChild<T>();
                if (desendent != null)
                {
                    return desendent;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据泛型类型获取父元素
        /// </summary>
        public static T? GetParent<T>(this DependencyObject source) where T : DependencyObject
        {
            while (true)
            {
                if (source is null)
                {
                    return null;
                }

                if (source is T st)
                {
                    return st;
                }

                DependencyObject parent = VisualTreeHelper.GetParent(source);

                if (parent is T pt)
                {
                    return pt;
                }

                source = parent;
            }
        }

        public static DependencyObject GetParent(this DependencyObject source)
        {
            return VisualTreeHelper.GetParent(source);
        }
    }
}
