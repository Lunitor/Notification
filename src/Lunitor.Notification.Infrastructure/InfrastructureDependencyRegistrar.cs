﻿using Lunitor.Notification.Core;
using Lunitor.Notification.Core.Repository;
using Lunitor.Notification.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Lunitor.Notification.Infrastructure
{
    public class InfrastructureDependencyRegistrar : DependencyRegistrar
    {
        public override void RegisterDependencies(IServiceCollection services)
        {
            services.AddScoped<IEmailContextProvider, EmailContextProvider>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}