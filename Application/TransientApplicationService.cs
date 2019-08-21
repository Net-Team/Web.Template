using Core;

namespace Application
{
    /// <summary>
    /// 表示Transient模式注册的应用服务抽象类
    /// 每次获取产生一个实例
    /// </summary>
    public abstract class TransientApplicationService : ApplicationService, ITransientDependency
    {
    }
}
