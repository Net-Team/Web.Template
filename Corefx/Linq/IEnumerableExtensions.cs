using System.Collections.Generic;

namespace System.Linq
{
    /// <summary>
    /// 可迭代类扩展
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// 为Null则返回0条记录的迭代
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> NullThenEmpty<T>(this IEnumerable<T> source)
        {
            if (source != null)
            {
                return source;
            }
            return Enumerable.Empty<T>();
        }

        /// <summary>
        /// 迭代集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> item)
        {
            if (source == null || item == null)
            {
                return;
            }

            foreach (var value in source)
            {
                item(value);
            }
        }
    }
}
