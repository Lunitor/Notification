using Lunitor.Notification.Core.Model;
using System.Collections.Generic;

namespace Lunitor.Notification.Web.Endpoints.Email
{
    public class SendEmailResponse
    {
        public string Type { get; set; }
        public IEnumerable<SendingResult> Results{ get; set; }
    }
}
