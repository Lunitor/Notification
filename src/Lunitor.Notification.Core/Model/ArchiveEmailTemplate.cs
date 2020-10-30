using System;
using System.Collections.ObjectModel;

namespace Lunitor.Notification.Core.Model
{
    public class ArchiveEmailTemplate
    {
        public DateTime TimeStamp { get; set; }
        public EmailTemplate EmailTemplate { get; set; }
        public ReadOnlyCollection<SendingResult> SendingResults { get; set; }
    }
}
