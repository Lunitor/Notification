using Lunitor.Notification.Core.Model;
using System.Collections.Generic;

namespace Lunitor.Notification.Core
{
    public interface IEmailCreator
    {
        IEnumerable<Email> CreateEmails(EmailTemplate template);
    }
}