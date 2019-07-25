using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System
{
    /// <summary>
    /// 提供Map扩展
    /// </summary>
    public static class MapExtensions
    {
        /// <summary>
        /// 转换为可映射对象
        /// 要求对象为public修饰
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IMap<TSource> AsMap<TSource>(this TSource value) where TSource : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return new Map<TSource>(value);
        }

        /// <summary>
        /// 转换为可映射对象
        /// 要求对象为public修饰
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="value"></param>
        /// <param name="includeMembers">要映射的成员名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IMap<TSource> AsMap<TSource>(this TSource value, params string[] includeMembers) where TSource : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return new Map<TSource>(value, includeMembers);
        }


        /// <summary>
        /// 表示映射体
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        private class Map<TSource> : IMap<TSource> where TSource : class
        {
            /// <summary>
            /// 数据源
            /// </summary>
            private readonly TSource source;

            /// <summary>
            /// 包含的属性名称
            /// </summary>
            private readonly HashSet<string> includeMembers;

            /// <summary>
            /// 源类型的所有属性
            /// </summary>
            private static readonly Property[] sourceProperies = Property.GetProperties(typeof(TSource));

            /// <summary>
            /// 源类型的所有属性名称
            /// </summary>
            private static string[] sourceMemberNames = sourceProperies.Select(item => item.Name).ToArray();

            /// <summary>
            /// 映射体
            /// </summary>
            /// <param name="source">数据源</param>
            /// <exception cref="ArgumentNullException"></exception>
            public Map(TSource source)
                : this(source, sourceMemberNames)
            {
            }

            /// <summary>
            /// 映射体
            /// </summary>
            /// <param name="source">数据源</param>
            /// <param name="includeMembers"></param>
            /// <exception cref="ArgumentNullException"></exception>
            public Map(TSource source, IEnumerable<string> includeMembers)
            {
                this.source = source ?? throw new ArgumentNullException(nameof(source));
                this.includeMembers = new HashSet<string>(includeMembers, StringComparer.OrdinalIgnoreCase);
            }

            /// <summary>
            /// 忽略映射的字段
            /// </summary>
            /// <typeparam name="TKey"></typeparam>
            /// <param name="ignoreKey">忽略的字段</param>
            /// <exception cref="ArgumentNullException"></exception>
            /// <returns></returns>
            public IMap<TSource> Ignore<TKey>(Expression<Func<TSource, TKey>> ignoreKey)
            {
                if (ignoreKey == null)
                {
                    throw new ArgumentNullException(nameof(ignoreKey));
                }

                if (ignoreKey.Body is MemberExpression body)
                {
                    this.includeMembers.Remove(body.Member.Name);
                }
                return this;
            }

            /// <summary>
            /// 忽略映射的字段
            /// </summary>
            /// <param name="memberName">忽略的字段</param>
            /// <returns></returns>
            public IMap<TSource> Ignore(params string[] memberName)
            {
                foreach (var item in memberName)
                {
                    this.includeMembers.Remove(item);
                }
                return this;
            }

            /// <summary>
            /// 映射到目标对象
            /// 要求destination为public修饰
            /// </summary>
            /// <typeparam name="TDestination"></typeparam>     
            /// <returns></returns>
            public TDestination To<TDestination>() where TDestination : class, new()
            {
                return this.To(new TDestination());
            }

            /// <summary>
            /// 映射到目标对象
            /// 要求destination为public修饰
            /// </summary>
            /// <typeparam name="TDestination"></typeparam>
            /// <param name="destination">目标对象</param>
            /// <returns></returns>
            public TDestination To<TDestination>(TDestination destination) where TDestination : class
            {
                if (destination == null)
                {
                    return null;
                }

                return MapItem<TDestination>.Map(this.source, destination, this.includeMembers);
            }

            /// <summary>
            /// 表示映射单元
            /// </summary>
            /// <typeparam name="TDestination"></typeparam>
            private static class MapItem<TDestination>
            {
                /// <summary>
                /// 属性名与属性操作映射表
                /// </summary>
                private static readonly Dictionary<string, MapProperty> mapTable;

                /// <summary>
                /// 静态构造器
                /// </summary>
                static MapItem()
                {
                    var q = from s in sourceProperies
                            join d in Property.GetProperties(typeof(TDestination))
                            on s.Name equals d.Name
                            where s.Info.CanRead && d.Info.CanWrite
                            select new MapProperty(s, d);

                    mapTable = q.ToDictionary(item => item.Name, item => item, StringComparer.OrdinalIgnoreCase);
                }

                /// <summary>
                /// 映射
                /// </summary>
                /// <param name="source">源</param>
                /// <param name="destination">目标</param>
                /// <param name="members">映射的属性</param>
                /// <returns></returns>
                public static TDestination Map(TSource source, TDestination destination, IEnumerable<string> members)
                {
                    foreach (var item in members)
                    {
                        if (mapTable.TryGetValue(item, out var map) == true)
                        {
                            var value = map.Source.GetValue(source);
                            map.Destination.SetValue(destination, value);
                        }
                    }
                    return destination;
                }
            }
        }


        /// <summary>
        /// 表示属性映射关系
        /// </summary>
        private class MapProperty
        {
            /// <summary>
            /// 获取属性名
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// 获取源属性
            /// </summary>
            public Property Source { get; }

            /// <summary>
            /// 获取目标属性
            /// </summary>
            public Property Destination { get; }

            /// <summary>
            /// 属性映射关系
            /// </summary>
            /// <param name="source">源属性</param>
            /// <param name="destination">目标属性</param>
            public MapProperty(Property source, Property destination)
            {
                this.Name = source.Name;
                this.Source = source;
                this.Destination = destination;
            }
        }
    }
}