using EmitMapper;
using EmitMapper.MappingConfiguration;
using Nelibur.ObjectMapper;
using System;
using System.Collections.Generic;
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
            private readonly TMap map;

            /// <summary>
            /// 忽略的字段
            /// </summary>
            private List<string> ignoreMembers;

            public Map(TMap map)
            {
                this.map = map;
            }

            /// <summary>
            /// 忽略映射的字段
            /// </summary>
            /// <typeparam name="TKey"></typeparam>
            /// <param name="ignore"></param>  
            /// <exception cref="ArgumentNullException"></exception>
            /// <returns></returns>
            public IMap<TMap> Ignore<TKey>(Expression<Func<TMap, TKey>> ignore)
            {
                if (ignore == null)
                {
                    throw new ArgumentNullException(nameof(ignore));
                }

                if (this.ignoreMembers == null)
                {
                    this.ignoreMembers = new List<string>();
                }

                if (ignore.Body is MemberExpression body)
                {
                    this.ignoreMembers.Add(body.Member.Name);
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

                if (this.ignoreMembers == null || this.ignoreMembers.Count == 0)
                {
                    return Tiny<TMap, TDestination>.Map(this.map, destination);
                }
                else
                {
                    var config = new DefaultMapConfig().IgnoreMembers<TMap, TDestination>(ignoreMembers.ToArray());
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
