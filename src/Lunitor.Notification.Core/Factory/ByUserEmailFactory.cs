using Lunitor.Notification.Core.Model;
using System.Collections.Generic;

namespace Lunitor.Notification.Core.Factory
{
    class ByUserEmailFactory : EmailFactory
    {
        public override IDictionary<string, string> Placeholders => new Dictionary<string, string>
        {
            { "UserName", "{USERNAME}" }
        };

        public override IEnumerable<Email> CreateEmails(EmailTemplateContent templateContent, EmailContext context)
        {
            var emails = new List<Email>();

            foreach (var user in context.Users)
            {
                var emailBody = templateContent.Text.Replace(Placeholders["UserName"], user.Name);
                var email = new Email
                {
                    ToAddress = user.EmailAddress,
                    Subject = templateContent.Subject,
                    Body = emailBody
                };

                emails.Add(email);
            }

            return emails;
        }
    }
}
