using Lunitor.Notification.Core.Model;
using System.Collections.Generic;

namespace Lunitor.Notification.Core.Repository
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();
    }
}
