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
                .Select(t => t.Name.Replace(nameof(EmailFactory), ""));
        }

        public IEnumerable<Type> GetAllTypes()
        {
            return _emailFactoryTypes;
        }
    }
}
