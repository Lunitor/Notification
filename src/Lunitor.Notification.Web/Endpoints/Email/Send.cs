using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Lunitor.Notification.Core;
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

        public Send(IEmailCreator emailCreator, IEmailSender emailSender)
        {
            Guard.Against.Null(emailCreator, nameof(emailCreator));
            Guard.Against.Null(emailSender, nameof(emailSender));

            _emailCreator = emailCreator;
            _emailSender = emailSender;
        }

        [HttpPost("/api/sendemail")]
        [Consumes("application/json")]
        public override async Task<ActionResult<SendEmailResponse>> HandleAsync(SendEmailRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var emails = await _emailCreator.CreateEmailsAsync(request.Map());

            await _emailSender.SendAsync(emails);

            return Ok(new SendEmailResponse
            {
                Type = request.Type,
                SentEmailCount = emails.Count()
            });
        }
    }
}
