using Lunitor.Notification.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lunitor.Notification.Core
{
    public interface IEmailCreator
    {
        Task<IEnumerable<Email>> CreateEmailsAsync(EmailTemplate template);
    }
}