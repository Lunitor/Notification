using Lunitor.Notification.Core.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lunitor.Notification.Core.Repository
{
    internal class EmailFactoryTypeRepository : IEmailFactoryTypeRepository
    {
        private readonly IEnumerable<Type> _emailFactoryTypes;

        public EmailFactoryTypeRepository()
        {
            _emailFactoryTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.BaseType == typeof(EmailFactory));
        }

        public IEnumerable<string> GetAllNames()
        {
            return _emailFactoryTypes
                .Select(t => CleanupName(t.Name));
        }

        public IEnumerable<Type> GetAllTypes()
        {
            return _emailFactoryTypes;
        }

        public IDictionary<string, string> GetPlaceholders(string factoryTypeName)
        {
            var factoryType = _emailFactoryTypes.FirstOrDefault(type => CleanupName(type.Name) == factoryTypeName.ToUpperInvariant());
            var factory = Activator.CreateInstance(factoryType) as EmailFactory;

            return factory.Placeholders;
        }

        private string CleanupName(string typeName)
        {
            return typeName.Replace(nameof(EmailFactory), "")
                .ToUpperInvariant();
        }
    }
}
