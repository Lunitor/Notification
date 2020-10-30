using System;

namespace Lunitor.Notification.Core.Model
{
    public class ArchiveEmailTemplate
    {
        public DateTime TimeStamp { get; set; }
        public EmailTemplate EmailTemplate { get; set; }
        public SendingResult[] SendingResults { get; set; }
    }
}
