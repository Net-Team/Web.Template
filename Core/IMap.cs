namespace Core
{
    /// <summary>
    /// 定义表示映射体的接口
    /// 提供对相同名称的属性进行映射
    /// </summary>
    /// <typeparam name="TMap"></typeparam>
    public interface IMap<TMap>
    {
        /// <summary>
        /// 映射到目标对象
        /// 要求destination为public修饰
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="destination">目标对象</param>
        /// <returns></returns>
        TDestination To<TDestination>(TDestination destination);

        /// <summary>
        /// 从其它对象映射过来
        /// 要求source为public修饰
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">来源</param>
        /// <returns></returns>
        TMap From<TSource>(TSource source);
    }
}
