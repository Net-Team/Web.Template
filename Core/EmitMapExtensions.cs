using EmitMapper;
using EmitMapper.MappingConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Core
{
    public static class EmitMapExtensions
    {
        /// <summary>
        /// 转换为可映射对象
        /// 要求对象为public修饰
        /// </summary>
        /// <typeparam name="TMap"></typeparam>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IMap<TMap> AsEmitMap<TMap>(this TMap value) where TMap : class
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
            private readonly IList<string> ignores = new List<string>();

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
                this.ignores.Add(((MemberExpression)ignore.Body).Member.Name);
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
                   
                var mapper = ObjectMapperManager.DefaultInstance.GetMapper<TMap, TDestination>();
                    return mapper.Map(map);
                }
                else
                {
                    var mapper = ObjectMapperManager.DefaultInstance.GetMapper<TMap, TDestination>(

                    new DefaultMapConfig()
                        .IgnoreMembers<TMap, TDestination>(ignores.ToArray()));

                    return mapper.Map(map);
                }
            }
        }
    }


}
