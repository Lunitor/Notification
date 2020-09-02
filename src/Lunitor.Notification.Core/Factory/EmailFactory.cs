using Lunitor.Notification.Core.Model;
using System.Collections.Generic;

namespace Lunitor.Notification.Core.Factory
{
    abstract class EmailFactory
    {
        public abstract IEnumerable<Email> CreateEmails(EmailTemplate template, EmailContext context);
    }
}
