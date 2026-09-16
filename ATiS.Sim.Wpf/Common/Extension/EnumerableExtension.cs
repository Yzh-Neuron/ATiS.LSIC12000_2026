using System.Linq;

namespace System.Collections.Generic
{
    public static class EnumerableExtension
    {
        public static IEnumerable<TSource> Do<TSource>(this IEnumerable<TSource> source, Action<TSource> predicate)
        {
            IList<TSource> enumerable = source as IList<TSource> ?? source.ToList();
            foreach (TSource item in enumerable)
            {
                predicate.Invoke(item);
            }

            return enumerable;
        }

        public static IEnumerable<TSource> DoWithIndex<TSource>(this IEnumerable<TSource> source, Action<TSource, int> predicate)
        {
            IList<TSource> enumerable = source as IList<TSource> ?? source.ToList();
            for (int i = 0; i < enumerable.Count; i++)
            {
                predicate.Invoke(enumerable[i], i);
            }

            return enumerable;
        }
    }
}
