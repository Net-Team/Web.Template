using Microsoft.Extensions.DependencyInjection;

namespace Core
{
    /// <summary>
    /// 定义Singleton模式的对象注入接口
    /// </summary>
    [Register(ServiceLifetime.Singleton)]
    public interface ISingletonDependency
    {
    }
}
