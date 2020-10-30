using Ardalis.GuardClauses;
using LiteDB;
using Lunitor.Notification.Core.Model;
using Lunitor.Notification.Core.Repository;
using Lunitor.Notification.Core.Utility;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Lunitor.Notification.Infrastructure.Repository
{
    internal class ArchiveEmailTemplateRepository : IArchiveEmailTemplateRepository
    {
        private readonly ILiteDatabase _database;
        private readonly IDateProvider _dateProvider;
        private readonly ILogger<ArchiveEmailTemplateRepository> _logger;

        public ArchiveEmailTemplateRepository(ILiteDbContext dbContext, IDateProvider dateProvider, ILogger<ArchiveEmailTemplateRepository> logger)
        {
            Guard.Against.Null(dbContext, nameof(dbContext));
            Guard.Against.Null(dateProvider, nameof(dateProvider));
            Guard.Against.Null(logger, nameof(logger));

            _database = dbContext.Database;
            _dateProvider = dateProvider;
            _logger = logger;
        }

        public IEnumerable<ArchiveEmailTemplate> GetAll()
        {
            return _database.GetCollection<ArchiveEmailTemplate>()
                .FindAll();
        }

        public void ArchiveEmailTemplate(EmailTemplate emailTemplate, List<SendingResult> results)
        {
            Guard.Against.Null(emailTemplate, nameof(emailTemplate));
            Guard.Against.Null(results, nameof(results));

            var archiveEmailTemplate = new ArchiveEmailTemplate
            {
                TimeStamp = _dateProvider.Now,
                EmailTemplate = emailTemplate,
                SendingResults = results.ToArray()
            };

            var id = _database.GetCollection<ArchiveEmailTemplate>()
                .Insert(archiveEmailTemplate);

            _logger.LogInformation("Archived {entityname} with [{subject}] subject.", nameof(EmailTemplate), emailTemplate.Content.Subject);
        }

    }
}
