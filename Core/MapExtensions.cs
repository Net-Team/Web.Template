using Nelibur.ObjectMapper;
using System;

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
            /// 初始化绑定
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
                return TinyMapper.Map(source, destination);
            }
        }

        /// <summary>
        /// IMap的默认实现者
        /// </summary>
        /// <typeparam name="TMap"></typeparam>
        private class Map<TMap> : IMap<TMap> where TMap : class
        {
            private readonly TMap map;

            public Map(TMap map)
            {
                this.map = map;
            }

            /// <summary>
            /// 从其它对象映射过来
            /// 要求source为public修饰
            /// </summary>
            /// <typeparam name="TSource"></typeparam>
            /// <param name="source">来源</param>
            /// <returns></returns>
            public TMap From<TSource>(TSource source) where TSource : class
            {
                if (source == null)
                {
                    return this.map;
                }
                return Mapper<TSource, TMap>.Map(source, this.map);
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
                return Mapper<TMap, TDestination>.Map(this.map, destination);
            }
        }
    }
}
