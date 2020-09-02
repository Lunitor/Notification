using System.Collections.Generic;

namespace Lunitor.Notification.Core.Model
{
    internal class EmailContext
    {
        public List<User> Users { get; private set; } = new List<User>();
    }
}