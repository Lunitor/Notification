using Lunitor.Notification.Core.Model;

namespace Lunitor.Notification.Core
{
    public interface IEmailContextProvider
    {
        EmailContext GetEmailContext();
    }
}
