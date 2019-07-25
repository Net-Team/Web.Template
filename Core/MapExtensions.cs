using EmitMapper;
using EmitMapper.MappingConfiguration;
using Nelibur.ObjectMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Core
{
    /// <summary>
    /// 提供映射扩展
    /// </summary>
    public static class MapExtensions
    {
        /// <summary>
        /// 转换为可映射对象
        /// 要求对象为public修饰
        /// </summary>
        /// <typeparam name="TMap"></typeparam>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IMap<TMap> AsMap<TMap>(this TMap value) where TMap : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            return new Map<TMap>(value);
        }

        /// <summary>
        /// 转换为可映射对象
        /// 要求对象为public修饰
        /// </summary>
        /// <typeparam name="TMap"></typeparam>
        /// <param name="value"></param>
        /// <param name="includeMembers">要映射的成员名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IMap<TMap> AsMap<TMap>(this TMap value, IEnumerable<string> includeMembers) where TMap : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (includeMembers == null || includeMembers.Any() == false)
            {
                return new Map<TMap>(value);
            }
            else
            {
                var ignoreMembers = Property<TMap>.MemberNames.Except(includeMembers);
                return new Map<TMap>(value, ignoreMembers);
            }
        }

        /// <summary>
        /// 提供类型的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static class Property<T>
        {
            /// <summary>
            /// 获取类型的所有属性名称
            /// </summary>
            public static string[] MemberNames { get; } = typeof(T).GetProperties().Select(item => item.Name).ToArray();
        }


        /// <summary>
        /// IMap的默认实现者
        /// </summary>
        /// <typeparam name="TMap"></typeparam>
        private class Map<TMap> : IMap<TMap> where TMap : class
        {
            /// <summary>
            /// 映射源
            /// </summary>
            private readonly TMap map;

            /// <summary>
            /// 忽略的字段
            /// </summary>
            private readonly HashSet<string> ignoreMembers = new HashSet<string>(StringComparer.InvariantCulture);

            /// <summary>
            /// IMap的默认实现者
            /// </summary>
            /// <param name="map"></param>
            public Map(TMap map)
                : this(map, null)
            {
            }

            /// <summary>
            /// IMap的默认实现者
            /// </summary>
            /// <param name="map"></param>
            /// <param name="ignores">忽略的字段</param>
            public Map(TMap map, IEnumerable<string> ignores)
            {
                this.map = map;
                ignores.ForEach(i => this.ignoreMembers.Add(i));
            }

            /// <summary>
            /// 忽略映射的字段
            /// </summary>
            /// <typeparam name="TKey"></typeparam>
            /// <param name="ignoreKey"></param>  
            /// <exception cref="ArgumentNullException"></exception>
            /// <returns></returns>
            public IMap<TMap> Ignore<TKey>(Expression<Func<TMap, TKey>> ignoreKey)
            {
                if (ignoreKey == null)
                {
                    throw new ArgumentNullException(nameof(ignoreKey));
                }

                if (ignoreKey.Body is MemberExpression body)
                {
                    this.ignoreMembers.Add(body.Member.Name);
                }
                return this;
            }

            /// <summary>
            /// 忽略映射的字段
            /// </summary>
            /// <param name="memberName">忽略的字段</param>
            /// <returns></returns>
            public IMap<TMap> Ignore(params string[] memberName)
            {
                foreach (var item in memberName)
                {
                    this.ignoreMembers.Add(item);
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

                if (this.ignoreMembers.Count == 0)
                {
                    return Tiny<TMap, TDestination>.Map(this.map, destination);
                }
                else
                {
                    var config = new DefaultMapConfig().IgnoreMembers<TMap, TDestination>(this.ignoreMembers.ToArray());
                    return ObjectMapperManager.DefaultInstance.GetMapper<TMap, TDestination>(config).Map(this.map);
                }
            }
        }


        /// <summary>
        /// 提供TinyMapper的静态初始化与映射操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        private static class Tiny<TSource, TDestination>
        {
            /// <summary>
            /// 静态初始化NoneIgnoreBinding
            /// </summary>
            static Tiny()
            {
                TinyMapper.Bind<TSource, TDestination>();
            }

            /// <summary>
            /// 映射对象
            /// </summary>
            /// <param name="source"></param>
            /// <param name="destination"></param>
            /// <returns></returns>
            public static TDestination Map(TSource source, TDestination destination)
            {
                return TinyMapper.Map(source, destination);
            }
        }

    }
}
