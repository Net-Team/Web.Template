using Core.Dependency;

namespace Application
{
    /// <summary>
    /// 表示Singleton模式注册的应用服务抽象类
    /// </summary>
    public abstract class SingletonApplicationService : ApplicationService, ISingletonDependency
    {
    }
}
