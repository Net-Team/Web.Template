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

        /// <summary>
        /// 以数字分段的排序
        /// </summary>
        /// <typeparam name="TRoom"></typeparam>
        /// <param name="source"></param>
        /// <param name="nameSelector">名称选择器</param>
        /// <returns></returns>
        public static IEnumerable<TRoom> OrderByNumericSegments<TRoom>(this IEnumerable<TRoom> source, Func<TRoom, string> nameSelector)
        {
            if (source == null)
            {
                return null;
            }

            return source
                .Select(item => new Segment<TRoom>(item, nameSelector(item)))
                .OrderBy(item => item.SegmentCount)
                .ThenBy(item => item.SegmentValue)
                .Select(item => item.Data);
        }

        /// <summary>
        /// 表示结构分段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class Segment<T>
        {
            /// <summary>
            /// 获取原始数据
            /// </summary>
            public T Data { get; private set; }

            /// <summary>
            /// 获取分段数量
            /// </summary>
            public int SegmentCount { get; private set; }

            /// <summary>
            /// 获取分段值
            /// </summary>
            public ulong SegmentValue { get; private set; }

            /// <summary>
            /// 项目的结构分段
            /// </summary>
            /// <param name="data">数据</param>
            /// <param name="name">名称</param>
            public Segment(T data, string name)
            {
                this.Data = data;
                var segments = name.Matches(@"\d+(?=\D)");
                this.SegmentCount = segments.Length;
                if (segments.Length > 0)
                {
                    this.SegmentValue = ulong.Parse(string.Join(string.Empty, segments));
                }
            }
        }
    }
}
