using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lunitor.Notification.Core.Factory
{
    internal class EmailFactoryProducer : IEmailFactoryProducer
    {
        private readonly IEnumerable<Type> _emailFactoryTypes;

        public EmailFactoryProducer()
        {
            _emailFactoryTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.BaseType == typeof(EmailFactory));
        }

        public EmailFactory GetEmailFactory(string templateType)
        {
            var emailFactoryType = _emailFactoryTypes
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