using LiteDB;
using Lunitor.Notification.Core.Model;
using Lunitor.Notification.Core.Utility;
using Lunitor.Notification.Infrastructure.Repository;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Lunitor.Notification.Infrastructure.UnitTests.Repository
{
    public class ArchiveEmailTemplateRepositoryTests
    {
        private ArchiveEmailTemplateRepository _archiveEmailTemplateRepository;

        private Mock<ILiteDbContext> _liteDbContextMock;
        private Mock<IDateProvider> _dateProviderStub;
        private Mock<ILogger<ArchiveEmailTemplateRepository>> _loggerMock;
        private Mock<ILiteDatabase> _liteDatabaseMock;

        private Mock<ILiteCollection<ArchiveEmailTemplate>> _archiveLiteCollectionMock;

        public ArchiveEmailTemplateRepositoryTests()
        {
            _liteDbContextMock = new Mock<ILiteDbContext>();
            _dateProviderStub = new Mock<IDateProvider>();
            _loggerMock = new Mock<ILogger<ArchiveEmailTemplateRepository>>();

            _liteDatabaseMock = new Mock<ILiteDatabase>();
            _archiveLiteCollectionMock = new Mock<ILiteCollection<ArchiveEmailTemplate>>();

            _liteDbContextMock.SetupGet(context => context.Database)
                .Returns(_liteDatabaseMock.Object);

            _liteDatabaseMock.Setup(database => database.GetCollection<ArchiveEmailTemplate>())
                .Returns(_archiveLiteCollectionMock.Object);

            _archiveLiteCollectionMock.Setup(collection => collection.Insert(It.IsAny<ArchiveEmailTemplate>()))
                .Returns(new Guid());

            _archiveEmailTemplateRepository = new ArchiveEmailTemplateRepository(_liteDbContextMock.Object, _dateProviderStub.Object, _loggerMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfLiteDbContextIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ArchiveEmailTemplateRepository(null, _dateProviderStub.Object, _loggerMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfDateProviderIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ArchiveEmailTemplateRepository(_liteDbContextMock.Object, null, _loggerMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ArchiveEmailTemplateRepository(_liteDbContextMock.Object, _dateProviderStub.Object, null));
        }

        [Fact]
        public void GetAll_ReturnsEmptyCollection_IfThereAreZeroArchiveEmailTemplateInTheDatabase()
        {
            // Arrange
            _archiveLiteCollectionMock.Setup(collection => collection.FindAll())
                .Returns(new List<ArchiveEmailTemplate>());

            // Act
            var archiveEmailTemplates = _archiveEmailTemplateRepository.GetAll();

            // Assert
            Assert.Empty(archiveEmailTemplates);
        }

        [Fact]
        public void GetAll_ReturnsXArchiveEmailTemplate_IfThereAreXArchiveEmailTemplateInTheDatabase()
        {
            // Arrange
            var archiveEmailTemplatesInDatabase = new List<ArchiveEmailTemplate>()
            {
                new ArchiveEmailTemplate(),
                new ArchiveEmailTemplate()
            };

            _archiveLiteCollectionMock.Setup(collection => collection.FindAll())
                .Returns(archiveEmailTemplatesInDatabase);

            // Act
            var archiveEmailTemplates = _archiveEmailTemplateRepository.GetAll();

            // Assert
            Assert.Equal(archiveEmailTemplatesInDatabase.Count, archiveEmailTemplates.Count());
        }

        [Fact]
        public void ArchiveEmailTemplate_ThrowsArgumentNullException_IfEmailTemplateIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _archiveEmailTemplateRepository.ArchiveEmailTemplate(null, new List<SendingResult>()));
        }

        [Fact]
        public void ArchiveEmailTemplate_ThrowsArgumentNullException_IfSendingResultsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _archiveEmailTemplateRepository.ArchiveEmailTemplate(new EmailTemplate(), null));
        }

        [Fact]
        public void ArchiveEmailTemplate_SavesAnArchiveEmailTemplateToDatabase()
        {
            _archiveEmailTemplateRepository.ArchiveEmailTemplate(new EmailTemplate(), new List<SendingResult>());

            _archiveLiteCollectionMock.Verify(collection =>
                collection.Insert(It.IsAny<ArchiveEmailTemplate>()),
                Times.Once);
        }

        [Fact]
        public void ArchiveEmailTemplate_SavesAnArchiveEmailTemplateWithTheGivenEmailTemplateAndSendingResults()
        {
            // Arrange
            var archiveEmailTemplatesInDatabase = new List<ArchiveEmailTemplate>();

            _archiveLiteCollectionMock.Setup(collection => collection.Insert(It.IsAny<ArchiveEmailTemplate>()))
                .Returns((ArchiveEmailTemplate entity) =>
                {
                    archiveEmailTemplatesInDatabase.Add(entity);

                    return new Guid();
                });

            var emailTemplate = new EmailTemplate();
            var sendingResults = new List<SendingResult>();

            // Act
            _archiveEmailTemplateRepository.ArchiveEmailTemplate(emailTemplate, sendingResults);

            // Assert
            Assert.Equal(emailTemplate, archiveEmailTemplatesInDatabase.First().EmailTemplate);
            Assert.Equal(sendingResults, archiveEmailTemplatesInDatabase.First().SendingResults);
        }

        [Fact]
        public void ArchiveEmailTemplate_SavesAnArchiveEmailTemplateWithCorrectTimeStamp()
        {
            // Arrange
            var archiveEmailTemplatesInDatabase = new List<ArchiveEmailTemplate>();

            var timeStamp = DateTime.Now;
            _dateProviderStub.SetupGet(provider => provider.Now)
                .Returns(timeStamp);

            _archiveLiteCollectionMock.Setup(collection => collection.Insert(It.IsAny<ArchiveEmailTemplate>()))
                .Returns((ArchiveEmailTemplate entity) =>
                {
                    archiveEmailTemplatesInDatabase.Add(entity);

                    return new Guid();
                });

            // Act
            _archiveEmailTemplateRepository.ArchiveEmailTemplate(new EmailTemplate(), new List<SendingResult>());

            // Assert
            Assert.Equal(timeStamp, archiveEmailTemplatesInDatabase.First().TimeStamp);
        }
    }
}
