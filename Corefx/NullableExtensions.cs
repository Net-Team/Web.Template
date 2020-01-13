namespace System
{
    /// <summary>
    /// 可空类型扩展
    /// </summary>
    public static class NullableExtensions
    {
        /// <summary>
        /// 如果为空则返回类型的初始值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T NullThenDefault<T>(this T? source) where T : struct
        {
            return source ?? default;
        }

        /// <summary>
        /// 如果为空则返回类型的初始值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T NullThen<T>(this T? source, T value) where T : struct
        {
            return source ?? value;
        }
    }
}
