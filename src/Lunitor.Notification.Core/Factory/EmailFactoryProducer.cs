using Ardalis.GuardClauses;
using Lunitor.Notification.Core.Repository;
using System;
using System.Linq;

namespace Lunitor.Notification.Core.Factory
{
    internal class EmailFactoryProducer : IEmailFactoryProducer
    {
        private readonly IEmailFactoryTypeRepository _emailFactoryTypeRepository;

        public EmailFactoryProducer(IEmailFactoryTypeRepository emailFactoryTypeRepository)
        {
            Guard.Against.Null(emailFactoryTypeRepository, nameof(emailFactoryTypeRepository));
            _emailFactoryTypeRepository = emailFactoryTypeRepository;
        }

        public EmailFactory GetEmailFactory(string templateType)
        {
            var emailFactoryType = _emailFactoryTypeRepository
                .GetAllTypes()
                .FirstOrDefault(factoryType => MathcingWithTemplateType(templateType, factoryType))
                ?? throw new ArgumentOutOfRangeException(templateType);

            return Activator.CreateInstance(emailFactoryType) as EmailFactory;
        }

        private static bool MathcingWithTemplateType(string templateType, Type factoryType)
        {
            return factoryType.Name.Replace(nameof(EmailFactory), "").ToUpperInvariant() == templateType.ToUpperInvariant();
        }
    }
}