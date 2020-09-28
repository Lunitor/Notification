using Lunitor.Notification.Core.Model;
using System.Collections.Generic;

namespace Lunitor.Notification.Core.Factory
{
    internal class CommonEmailFactory : EmailFactory
    {
        public override IDictionary<string, string> Placeholders => new Dictionary<string, string>();

        public override IEnumerable<Email> CreateEmails(EmailTemplateContent templateContent, EmailContext context)
        {
            var emails = new List<Email>();

            var email = new Email
            {
                Subject = templateContent.Subject,
                Body = templateContent.Text
            };

            foreach (var user in context.Users)
            {
                email.BCCAddresses.Add(user.EmailAddress);
            }

            emails.Add(email);

            return emails;
        }
    }
}
