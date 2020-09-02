using Lunitor.Notification.Core.Model;

namespace Lunitor.Notification.Core
{
    interface IEmailContextProvider
    {
        EmailContext GetEmailContext();
    }
}
