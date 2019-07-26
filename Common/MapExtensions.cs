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
        /// 类型转换方法
        /// </summary>
        private static readonly MethodInfo convertToTypeMethod = typeof(Converter).GetMethod($"{nameof(Converter.ConvertToType)}", BindingFlags.Static | BindingFlags.Public);

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
            private static readonly PropertyInfo[] sourceProperies = typeof(TSource).GetProperties();

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
                            join d in typeof(TDestination).GetProperties()
                            on s.Name equals d.Name
                            where s.CanRead && d.CanWrite
                            select new MapProperty(s.Name);

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
                            map.Invoke(source, destination);
                        }
                    }
                    return destination;
                }

                /// <summary>
                /// 表示映射属性
                /// </summary>
                private class MapProperty
                {
                    /// <summary>
                    /// 映射委托
                    /// </summary>
                    private readonly Action<TSource, TDestination> mapAction;

                    /// <summary>
                    /// 获取属性名称
                    /// </summary>
                    public string Name { get; }

                    /// <summary>
                    /// 映射属性
                    /// </summary>
                    /// <param name="name">属性名称</param>
                    public MapProperty(string name)
                    {
                        this.Name = name;
                        this.mapAction = CreateMapAction(name);
                    }

                    /// <summary>
                    /// 创建映射委托
                    /// (source,destination) => source.Name =  destination.Name;
                    /// </summary>
                    /// <param name="name">属性名</param>
                    /// <returns></returns>
                    private static Action<TSource, TDestination> CreateMapAction(string name)
                    {
                        var parameterSource = Expression.Parameter(typeof(TSource), "source");
                        var parameterDestination = Expression.Parameter(typeof(TDestination), "destination");

                        var propertySource = typeof(TSource).GetProperty(name);
                        var propertyDestination = typeof(TDestination).GetProperty(name);

                        var value = (Expression)Expression.Property(parameterSource, name);
                        if (propertySource.PropertyType != propertyDestination.PropertyType)
                        {
                            var valueArg = Expression.Convert(value, typeof(object));
                            var targetTypeArg = Expression.Constant(propertyDestination.PropertyType);
                            var objectValue = Expression.Call(null, convertToTypeMethod, valueArg, targetTypeArg);
                            value = Expression.Convert(objectValue, propertyDestination.PropertyType);
                        }

                        var body = Expression.Call(parameterDestination, propertyDestination.GetSetMethod(), value);
                        return Expression.Lambda<Action<TSource, TDestination>>(body, parameterSource, parameterDestination).Compile();
                    }

                    /// <summary>
                    /// 执行映射
                    /// </summary>
                    /// <param name="source">源</param>
                    /// <param name="destination">目标</param>
                    public void Invoke(TSource source, TDestination destination)
                    {
                        this.mapAction.Invoke(source, destination);
                    }

                    /// <summary>
                    /// 转换为字符串
                    /// </summary>
                    /// <returns></returns>
                    public override string ToString()
                    {
                        return this.Name;
                    }
                }
            }
        }
    }
}