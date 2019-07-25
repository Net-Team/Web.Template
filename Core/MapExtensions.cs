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
            private readonly Lazy<HashSet<string>> ignoreMembers = new Lazy<HashSet<string>>(() => new HashSet<string>(StringComparer.InvariantCulture));

            /// <summary>
            /// 包含的字段
            /// </summary>
            private readonly Lazy<HashSet<string>> includeMembers = new Lazy<HashSet<string>>(() => new HashSet<string>(StringComparer.InvariantCulture));

            /// <summary>
            /// IMap的默认实现者
            /// </summary>
            /// <param name="map"></param>
            public Map(TMap map)
            {
                this.map = map;
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
                    this.ignoreMembers.Value.Add(body.Member.Name);
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
                    this.ignoreMembers.Value.Add(item);
                }
                return this;
            }

            /// <summary>
            /// 设置要映射的字段名
            /// 留空表示全部字段
            /// </summary>
            /// <typeparam name="TKey"></typeparam>
            /// <param name="includeKey">包含的字段</param>
            /// <exception cref="ArgumentNullException"></exception>
            /// <returns></returns>
            public IMap<TMap> Include<TKey>(Expression<Func<TMap, TKey>> includeKey)
            {
                if (includeKey == null)
                {
                    throw new ArgumentNullException(nameof(includeKey));
                }

                if (includeKey.Body is MemberExpression body)
                {
                    this.includeMembers.Value.Add(body.Member.Name);
                }
                return this;
            }


            /// <summary>
            /// 设置要映射的字段名
            /// 留空表示全部字段
            /// </summary>
            /// <param name="members"></param>
            /// <returns></returns>
            public IMap<TMap> Include(params string[] members)
            {
                if (members == null)
                {
                    return this;
                }

                foreach (var m in members)
                {
                    this.includeMembers.Value.Add(m);
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

                var ignores = this.GetIgnoreMembers();
                if (ignores == null || ignores.Length == 0)
                {
                    return Tiny<TMap, TDestination>.Map(this.map, destination);
                }
                else
                {
                    var config = new DefaultMapConfig().IgnoreMembers<TMap, TDestination>(ignores);
                    return ObjectMapperManager.DefaultInstance.GetMapper<TMap, TDestination>(config).Map(this.map);
                }
            }

            /// <summary>
            /// 获取忽略的属性
            /// </summary>
            /// <returns></returns>
            private string[] GetIgnoreMembers()
            {
                if (this.ignoreMembers.IsValueCreated == false && this.includeMembers.IsValueCreated == false)
                {
                    return null;
                }

                if (this.ignoreMembers.IsValueCreated == true)
                {
                    return this.ignoreMembers.Value.ToArray();
                }

                if (this.includeMembers.IsValueCreated == true)
                {
                    return Property<TMap>.MemberNames
                        .Except(this.includeMembers.Value)
                        .ToArray();
                }

                return Property<TMap>.MemberNames
                    .Except(this.includeMembers.Value)
                    .Union(this.ignoreMembers.Value)
                    .ToArray();
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
