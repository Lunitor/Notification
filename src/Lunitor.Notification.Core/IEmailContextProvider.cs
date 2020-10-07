using Lunitor.Notification.Core.Model;
using System.Threading.Tasks;

namespace Lunitor.Notification.Core
{
    public interface IEmailContextProvider
    {
        Task<EmailContext> GetEmailContextAsync();
    }
}
