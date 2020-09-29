using Lunitor.Notification.Core.Factory;
using Lunitor.Notification.Core.Repository;
using Lunitor.Notification.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Lunitor.Notification.Core
{
    public class CoreDependecyRegistrar : DependencyRegistrar
    {
        public override void RegisterDependencies(IServiceCollection services)
        {
            services.AddScoped<IEmailFactoryTypeRepository, EmailFactoryTypeRepository>();
            services.AddScoped<IEmailCreator, EmailCreator>();
            services.AddScoped<IEmailFactoryProducer, EmailFactoryProducer>();
        }
    }
}
