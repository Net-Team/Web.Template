using System;
using System.Linq.Expressions;

namespace Core
{
    /// <summary>
    /// 定义表示映射体的接口
    /// 提供对相同名称的属性进行映射
    /// </summary>
    /// <typeparam name="TMap"></typeparam>
    public interface IMap<TMap> where TMap : class
    {
        /// <summary>
        /// 忽略映射的字段
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="ignoreKey">忽略的字段</param>
        /// <returns></returns>
        IMap<TMap> Ignore<TKey>(Expression<Func<TMap, TKey>> ignoreKey);

        /// <summary>
        /// 映射到目标对象
        /// 要求destination为public修饰
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>     
        /// <returns></returns>
        TDestination To<TDestination>() where TDestination : class, new();

        /// <summary>
        /// 映射到目标对象
        /// 要求destination为public修饰
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="destination">目标对象</param>
        /// <returns></returns>
        TDestination To<TDestination>(TDestination destination) where TDestination : class;
    }
}
