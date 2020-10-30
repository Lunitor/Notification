using Lunitor.Notification.Core;
using Lunitor.Notification.Core.Repository;
using Lunitor.Notification.Core.Utility;
using Lunitor.Notification.Infrastructure.Repository;
using Lunitor.Notification.Infrastructure.Utility;
using Lunitor.Notification.Shared;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;

namespace Lunitor.Notification.Infrastructure
{
    public class InfrastructureDependencyRegistrar : DependencyRegistrar
    {
        public override void RegisterDependencies(IServiceCollection services)
        {
            services.AddScoped<IEmailContextProvider, EmailContextProvider>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddTransient<ISmtpClient, SmtpClient>();

            services.AddHttpClient<IUserRepository, JellyfinUserRepository>();

            services.AddSingleton<IDateProvider, SystemDateProvider>();

            services.AddSingleton<ILiteDbContext, LiteDbContext>();
            services.AddTransient<IArchiveEmailTemplateRepository, ArchiveEmailTemplateRepository>();
        }
    }
}
