using System.Collections.Generic;

namespace Lunitor.Notification.Core.Model
{
    internal class Email
    {
        public string ToAddresses { get; set; }
        public List<string> BCCAddresses { get; private set; } = new List<string>();
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}