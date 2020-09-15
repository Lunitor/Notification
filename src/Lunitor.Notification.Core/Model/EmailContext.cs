using System.Collections.Generic;

namespace Lunitor.Notification.Core.Model
{
    public class EmailContext
    {
        public List<User> Users { get; private set; } = new List<User>();
    }
}