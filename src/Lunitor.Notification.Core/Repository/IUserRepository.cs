using Lunitor.Notification.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lunitor.Notification.Core.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
    }
}
