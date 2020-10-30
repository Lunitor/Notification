using Lunitor.Notification.Core.Model;
using System.Collections.Generic;

namespace Lunitor.Notification.Core.Repository
{
    public interface IArchiveEmailTemplateRepository
    {
        public IEnumerable<ArchiveEmailTemplate> GetAll();
        public void ArchiveEmailTemplate(EmailTemplate emailTemplate, List<SendingResult> results);
    }
}
