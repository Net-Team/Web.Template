using Core;

namespace Application
{
    /// <summary>
    /// 表示Scoped模式注册的应用服务抽象类
    /// 每个请求产生一个实例
    /// </summary>
    public abstract class ScopedApplicationService : ApplicationService, IScopedDependency
    {
    }
}
