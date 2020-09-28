using Lunitor.Notification.Core.Model;

namespace Lunitor.Notification.Web.Endpoints.Email
{
    public class SendEmailRequest
    {
        public string Type { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public EmailTemplate Map()
        {
            return new EmailTemplate(Type, Subject, Body);
        }
    }
}
