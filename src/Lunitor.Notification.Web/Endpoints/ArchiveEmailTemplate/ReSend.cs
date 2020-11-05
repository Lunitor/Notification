using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Lunitor.Notification.Core;
using Lunitor.Notification.Core.Model;
using Lunitor.Notification.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lunitor.Notification.Web.Endpoints.ArchiveEmailTemplate
{
    public class ReSend : BaseAsyncEndpoint<ReSendRequest, IEnumerable<SendingResult>>
    {
        private readonly IArchiveEmailTemplateRepository _archiveEmailTemplateRepository;
        private readonly IEmailCreator _emailCreator;
        private readonly IEmailSender _emailSender;

        public ReSend(IArchiveEmailTemplateRepository archiveEmailTemplateRepository,
            IEmailCreator emailCreator,
            IEmailSender emailSender)
        {
            Guard.Against.Null(archiveEmailTemplateRepository, nameof(archiveEmailTemplateRepository));
            Guard.Against.Null(emailCreator, nameof(emailCreator));
            Guard.Against.Null(emailSender, nameof(emailSender));

            _archiveEmailTemplateRepository = archiveEmailTemplateRepository;
            _emailCreator = emailCreator;
            _emailSender = emailSender;
        }

        public override async Task<ActionResult<IEnumerable<SendingResult>>> HandleAsync(ReSendRequest request, CancellationToken cancellationToken = default)
        {
            var archiveEmailTemplate = _archiveEmailTemplateRepository.GetAll()
                .FirstOrDefault(archiveEmailTemplate => archiveEmailTemplate.TimeStamp == request.ArchivedEmailTimeStamp);

            if (archiveEmailTemplate == null)
            {
                return NotFound($"There is no archived email template with {request.ArchivedEmailTimeStamp} timestamp");
            }

            if (archiveEmailTemplate.SendingResults.Count(sendingResult => sendingResult.IsSuccess == false) == 0)
            {
                return NotFound($"There is no unsuccessfull email sending for template with {request.ArchivedEmailTimeStamp} timestamp");
            }

            var emails = await _emailCreator.CreateEmailsAsync(archiveEmailTemplate.EmailTemplate);
            if (emails.Count() == 1 && emails.FirstOrDefault()?.BCCAddresses.Count > 0)
            {
                return BadRequest($"Can't resend emails with BCC addresses");
            }

            var toBeReSendEmails = FilterToBeReSendEmails(archiveEmailTemplate, emails);
            var results = await _emailSender.SendAsync(toBeReSendEmails);

            return Ok(results);
        }

        private static IEnumerable<Core.Model.Email> FilterToBeReSendEmails(Core.Model.ArchiveEmailTemplate archiveEmailTemplate, IEnumerable<Core.Model.Email> emails)
        {
            var failedEmailAddresses = archiveEmailTemplate.SendingResults
                .Where(sendingResult => sendingResult.IsSuccess == false)
                .Select(sendingResult => sendingResult.EmailAddress);

            return emails.Where(email => failedEmailAddresses.Contains(email.ToAddress));
        }
    }
}
