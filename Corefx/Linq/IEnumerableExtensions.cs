using System.Collections.Generic;

namespace System.Linq
{
    /// <summary>
    /// 可迭代类扩展
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// 去除重复数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            return source.Distinct(keySelector, EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// 去除重复数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="keyComparer"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            if (keyComparer == null)
            {
                throw new ArgumentNullException(nameof(keyComparer));
            }
            var comparer = new KeyEqualityComparer<T, TKey>(keySelector, keyComparer);
            return source.Distinct(comparer);
        }

        /// <summary>
        /// 为Null则返回0条记录的迭代
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> NullThenEmpty<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
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

        /// <summary>
        /// 以数字分段的排序
        /// </summary>
        /// <typeparam name="TRoom"></typeparam>
        /// <param name="source"></param>
        /// <param name="nameSelector">名称选择器</param>
        /// <returns></returns>
        public static IOrderedEnumerable<TRoom> OrderByNumericSegments<TRoom>(this IEnumerable<TRoom> source, Func<TRoom, string> nameSelector)
        {
            if (source == null)
            {
                return null;
            }

            return source.OrderBy(nameSelector, NumericSegmentComparer.Instance);
        }

        /// <summary>
        /// 键选择比较器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        private class KeyEqualityComparer<T, TKey> : IEqualityComparer<T>
        {
            private readonly Func<T, TKey> keySelector;
            private readonly IEqualityComparer<TKey> comparer;

            public KeyEqualityComparer(Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer)
            {
                this.keySelector = keySelector;
                this.comparer = comparer;
            }

            public bool Equals(T x, T y)
            {
                return comparer.Equals(keySelector(x), keySelector(y));
            }

            public int GetHashCode(T obj)
            {
                return comparer.GetHashCode(keySelector(obj));
            }
        }



        /// <summary>
        /// 比较器
        /// </summary>
        private class NumericSegmentComparer : IComparer<string>
        {
            private const string Pattern = "\\d+(?=\\D)";

            public static IComparer<string> Instance { get; } = new NumericSegmentComparer();

            public int Compare(string x, string y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }

                if (x == null)
                {
                    return -1;
                }

                if (y == null)
                {
                    return 1;
                }

                var xSegments = x.Matches(Pattern);
                var ySegments = y.Matches(Pattern);

                var c = xSegments.Length - ySegments.Length;
                if (c != 0)
                {
                    return c;
                }

                for (var i = 0; i < xSegments.Length; i++)
                {
                    x = xSegments[i];
                    y = ySegments[i];

                    c = int.Parse(x) - int.Parse(y);
                    if (c != 0)
                    {
                        return c;
                    }
                }

                return 0;
            }
        }
    }
}
