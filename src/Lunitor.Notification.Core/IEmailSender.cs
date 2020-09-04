using Lunitor.Notification.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lunitor.Notification.Core
{
    public interface IEmailSender
    {
        Task SendAsync(IEnumerable<Email> emails);
    }
}
