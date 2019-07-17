using Core.Dependency;

namespace Application
{
    public abstract class SingletonApplicationService : ApplicationService, ISingletonDependency
    {
    }
}
