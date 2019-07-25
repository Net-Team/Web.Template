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
        /// 提供TinyMapper的静态初始化与映射操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        private static class Mapper<TSource, TDestination>
        {
            /// <summary>
            /// 当前是否为有ignore的绑定
            /// </summary>
            private static bool hasIgnoreBinding = false;

            /// <summary>
            /// 排它锁
            /// </summary>
            private static object syncRoot = new object();

            /// <summary>
            /// 静态初始化NoneIgnoreBinding
            /// </summary>
            static Mapper()
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
                lock (syncRoot)
                {
                    if (hasIgnoreBinding == true)
                    {
                        TinyMapper.Bind<TSource, TDestination>();
                        hasIgnoreBinding = false;
                    }
                    return TinyMapper.Map(source, destination);
                }
            }

            /// <summary>
            /// 映射对象
            /// </summary>
            /// <param name="source"></param>
            /// <param name="destination"></param>
            /// <param name="ignores">忽略项</param>
            /// <returns></returns>
            public static TDestination Map(TSource source, TDestination destination, IEnumerable<Expression<Func<TSource, object>>> ignores)
            {
                lock (syncRoot)
                {
                    hasIgnoreBinding = true;
                    TinyMapper.Bind<TSource, TDestination>(c => ignores.ForEach(i => c.Ignore(i)));
                    return TinyMapper.Map(source, destination);
                }
            }
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
            private readonly IList<Expression<Func<TMap, object>>> ignores = new List<Expression<Func<TMap, object>>>();

            public Map(TMap map)
            {
                this.map = map;
            }

            /// <summary>
            /// 忽略映射的字段
            /// </summary>
            /// <typeparam name="TKey"></typeparam>
            /// <param name="ignore"></param>           
            /// <returns></returns>
            public IMap<TMap> Ignore<TKey>(Expression<Func<TMap, TKey>> ignore)
            {
                var body = Expression.Convert(ignore.Body, typeof(object));
                var expression = Expression.Lambda<Func<TMap, object>>(body, ignore.Parameters);
                this.ignores.Add(expression);
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

                if (this.ignores.Count == 0)
                {
                    return Mapper<TMap, TDestination>.Map(this.map, destination);
                }
                else
                {
                    return Mapper<TMap, TDestination>.Map(this.map, destination, this.ignores);
                }
            }
        }
    }
}
