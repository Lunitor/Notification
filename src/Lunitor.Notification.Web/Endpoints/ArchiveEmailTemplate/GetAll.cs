using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using Lunitor.Notification.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lunitor.Notification.Web.Endpoints.ArchiveEmailTemplate
{
    public class GetAll : BaseAsyncEndpoint<IEnumerable<Core.Model.ArchiveEmailTemplate>>
    {
        private readonly IArchiveEmailTemplateRepository _archiveEmailTemplateRepository;

        public GetAll(IArchiveEmailTemplateRepository archiveEmailTemplateRepository)
        {
            Guard.Against.Null(archiveEmailTemplateRepository, nameof(archiveEmailTemplateRepository));

            _archiveEmailTemplateRepository = archiveEmailTemplateRepository;
        }

        [HttpGet("/api/getarchiveemailtemplates")]
        public override async Task<ActionResult<IEnumerable<Core.Model.ArchiveEmailTemplate>>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var archiveEmailTemplates = _archiveEmailTemplateRepository.GetAll();

            return Ok(archiveEmailTemplates);
        }
    }
}
