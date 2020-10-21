using System;
using System.Collections.Generic;

namespace Lunitor.Notification.Core.Repository
{
    public interface IEmailFactoryTypeRepository
    {
        public IEnumerable<string> GetAllNames();
        public IEnumerable<Type> GetAllTypes();
        public IDictionary<string, string> GetPlaceholders(string factoryTypeName);
    }
}
