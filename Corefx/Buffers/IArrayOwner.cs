#if NETCOREAPP3_0
namespace System.Buffers
{
    /// <summary>
    /// 定义数组持有者的接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IArrayOwner<T> : IDisposable
    {
        /// <summary>
        /// 获取持有的数组
        /// </summary>
        T[] Array { get; }

        /// <summary>
        /// 获取数组的有效长度
        /// </summary>
        int Count { get; }
    }
}
#endif