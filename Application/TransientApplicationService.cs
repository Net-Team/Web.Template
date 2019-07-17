using Core.Dependency;

namespace Application
{
    public abstract class TransientApplicationService : ApplicationService, ITransientDependency
    {
    }
}
