using EmitMapper;
using EmitMapper.MappingConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Core
{
    /// <summary>
    /// 提供映射扩展
    /// </summary>
    public static class EmitMapExtensions
    {
        /// <summary>
        /// 转换为可映射对象
        /// 要求对象为public修饰
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IMap<TSource> AsEmitMap<TSource>(this TSource value) where TSource : class
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
        public static IMap<TSource> AsEmitMap<TSource>(this TSource value, IEnumerable<string> includeMembers) where TSource : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (includeMembers == null || includeMembers.Any() == false)
            {
                return new Map<TSource>(value);
            }
            else
            {
                var ignoreMembers = Map<TSource>.MemberNames.Except(includeMembers);
                return new Map<TSource>(value, ignoreMembers);
            }
        }


        /// <summary>
        /// IMap的默认实现者
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        private class Map<TSource> : IMap<TSource> where TSource : class
        {
            /// <summary>
            /// 映射源
            /// </summary>
            private readonly TSource source;

            /// <summary>
            /// 忽略的字段
            /// </summary>
            private readonly HashSet<string> ignoreMembers = new HashSet<string>(StringComparer.InvariantCulture);

            /// <summary>
            /// 获取类型的所有属性名称
            /// </summary>
            public static string[] MemberNames { get; } = typeof(TSource).GetProperties().Select(item => item.Name).ToArray();

            /// <summary>
            /// IMap的默认实现者
            /// </summary>
            /// <param name="source"></param>
            public Map(TSource source)
                : this(source, null)
            {
            }

            /// <summary>
            /// IMap的默认实现者
            /// </summary>
            /// <param name="source"></param>
            /// <param name="ignores">忽略的字段</param>
            public Map(TSource source, IEnumerable<string> ignores)
            {
                this.source = source;
                ignores.ForEach(i => this.ignoreMembers.Add(i));
            }

            /// <summary>
            /// 忽略映射的字段
            /// </summary>
            /// <typeparam name="TKey"></typeparam>
            /// <param name="ignoreKey"></param>  
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
                    this.ignoreMembers.Add(body.Member.Name);
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
                    return ObjectMapperManager.DefaultInstance.GetMapper<TSource, TDestination>().Map(this.source);
                }
                else
                {
                    var config = new DefaultMapConfig().IgnoreMembers<TSource, TDestination>(this.ignoreMembers.ToArray());
                    return ObjectMapperManager.DefaultInstance.GetMapper<TSource, TDestination>(config).Map(this.source);
                }
            }
        }
    }
}
