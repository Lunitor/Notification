using Lunitor.Notification.Core.Model;
using System.Collections.Generic;

namespace Lunitor.Notification.Core.Factory
{
    internal abstract class EmailFactory
    {
        public abstract IDictionary<string, string> Placeholders { get; }

        public abstract IEnumerable<Email> CreateEmails(EmailTemplateContent templateContent, EmailContext context);
    }
}
