using Lunitor.Notification.Core.Model;
using System.Collections.Generic;

namespace Lunitor.Notification.Core
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();
    }
}
