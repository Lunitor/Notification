using Microsoft.Extensions.DependencyInjection;

namespace Lunitor.Notification.Shared
{
    public abstract class DependencyRegistrar
    {
        public abstract void RegisterDependencies(IServiceCollection services);
    }
}
