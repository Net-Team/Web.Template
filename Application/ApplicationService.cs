using System;

namespace Application
{
    /// <summary>
    /// 表示服务抽象类
    /// </summary>
    public abstract class ApplicationService : Disposable
    {
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否也释放非托管资源 </param>
        protected override void Dispose(bool disposing)
        {
        }
    }
}
