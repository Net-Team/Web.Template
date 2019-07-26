using System;

namespace Web.Core
{
    /// <summary>
    /// 定义具有指定生命周期范围的服务提供者接口
    /// </summary>
    public interface IScopeServiceProvider : IServiceProvider, IDisposable
    {
    }
}
