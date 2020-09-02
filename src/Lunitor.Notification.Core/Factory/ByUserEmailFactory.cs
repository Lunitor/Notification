using Lunitor.Notification.Core.Model;
using System.Collections.Generic;

namespace Lunitor.Notification.Core.Factory
{
    class ByUserEmailFactory : EmailFactory
    {
        public override IEnumerable<Email> CreateEmails(EmailTemplate template, EmailContext context)
        {
            var emails = new List<Email>();

            foreach (var user in context.Users)
            {
                var email = new Email
                {
                    Subject = template.Subject,
                    Body = template.Text
                };

                email.ToAddresses = user.EmailAddress;
                emails.Add(email);
            }

            return emails;
        }
    }
}
