using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Lunitor.Notification.Core;
using Lunitor.Notification.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lunitor.Notification.Web.Endpoints.Email
{
    public class Send : BaseAsyncEndpoint<SendEmailRequest, SendEmailResponse>
    {
        private readonly IEmailCreator _emailCreator;
        private readonly IEmailSender _emailSender;
        private readonly IArchiveEmailTemplateRepository _archiveRepository;

        public Send(IEmailCreator emailCreator, IEmailSender emailSender, IArchiveEmailTemplateRepository archiveRepository)
        {
            Guard.Against.Null(emailCreator, nameof(emailCreator));
            Guard.Against.Null(emailSender, nameof(emailSender));
            Guard.Against.Null(archiveRepository, nameof(archiveRepository));

            _emailCreator = emailCreator;
            _emailSender = emailSender;
            _archiveRepository = archiveRepository;
        }

        [HttpPost("/api/sendemail")]
        [Consumes("application/json")]
        public override async Task<ActionResult<SendEmailResponse>> HandleAsync(SendEmailRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var emailTemplate = request.MapToEmailTemplate();
            var emails = await _emailCreator.CreateEmailsAsync(emailTemplate);

            var results = await _emailSender.SendAsync(emails);

            _archiveRepository.ArchiveEmailTemplate(emailTemplate, results.ToList());

            return Ok(new SendEmailResponse
            {
                Type = request.Type,
                Results = results
            });
        }
    }
}
