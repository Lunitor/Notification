using System.Collections.Generic;

namespace Lunitor.Notification.Core.Model
{
    public class Email
    {
        public string ToAddress { get; set; }
        public List<string> BCCAddresses { get; private set; } = new List<string>();
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}