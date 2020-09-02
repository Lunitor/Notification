using Lunitor.Notification.Core.Model;
using System.Collections.Generic;

namespace Lunitor.Notification.Core.Factory
{
    class CommonEmailFactory : EmailFactory
    {
        public override IEnumerable<Email> CreateEmails(EmailTemplate template, EmailContext context)
        {
            var emails = new List<Email>();

            var email = new Email
            {
                Subject = template.Subject,
                Body = template.Text
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
