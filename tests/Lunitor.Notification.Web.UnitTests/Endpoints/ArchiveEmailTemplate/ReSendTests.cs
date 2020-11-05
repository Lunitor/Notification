using Lunitor.Notification.Core;
using Lunitor.Notification.Core.Model;
using Lunitor.Notification.Core.Repository;
using Lunitor.Notification.Web.Endpoints.ArchiveEmailTemplate;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Lunitor.Notification.Web.UnitTests.Endpoints.ArchiveEmailTemplate
{
    public class ReSendTests
    {
        private ReSend _reSend;

        private Mock<IArchiveEmailTemplateRepository> _archiveEmailTemplateRepositoryMock;
        private Mock<IEmailCreator> _emailCreatorMock;
        private Mock<IEmailSender> _emailSenderMock;

        private const string TestEmailAddress = "test@test.com";

        public ReSendTests()
        {
            _archiveEmailTemplateRepositoryMock = new Mock<IArchiveEmailTemplateRepository>();
            _emailCreatorMock = new Mock<IEmailCreator>();
            _emailSenderMock = new Mock<IEmailSender>();

            _reSend = new ReSend(
                _archiveEmailTemplateRepositoryMock.Object,
                _emailCreatorMock.Object,
                _emailSenderMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfArchiveEmailTemplateRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ReSend(
                null,
                _emailCreatorMock.Object,
                _emailSenderMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfEmailCreatorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ReSend(
                _archiveEmailTemplateRepositoryMock.Object,
                null,
                _emailSenderMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfEmailSenderIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ReSend(
                _archiveEmailTemplateRepositoryMock.Object,
                _emailCreatorMock.Object,
                null));
        }

        [Fact]
        public async Task HandleAsync_ReturnsNotFound_IfArchivedEmailTimeStampNotFoundInArchiveEmailTemplateRepository()
        {
            _archiveEmailTemplateRepositoryMock.Setup(repository => repository.GetAll())
                .Returns(new List<Core.Model.ArchiveEmailTemplate>());

            var request = new ReSendRequest()
            {
                ArchivedEmailTimeStamp = DateTime.Now
            };

            var response = await _reSend.HandleAsync(request);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async Task HandleAsync_ReturnsBadRequest_IfGeneratedEmailHasBCCAddress()
        {
            // Arrange
            var timeStamp = DateTime.Now;

            _archiveEmailTemplateRepositoryMock.Setup(repository => repository.GetAll())
                .Returns(new List<Core.Model.ArchiveEmailTemplate>()
                {
                    new Core.Model.ArchiveEmailTemplate
                    {
                        TimeStamp = timeStamp,
                        SendingResults = new SendingResult[]
                        {
                            new SendingResult(TestEmailAddress, false)
                        }
                    }
                });

            var createdEmail = new Core.Model.Email();
            createdEmail.BCCAddresses.Add(TestEmailAddress);
            _emailCreatorMock.Setup(emailCreator => emailCreator.CreateEmailsAsync(It.IsAny<EmailTemplate>()))
                .ReturnsAsync(new List<Core.Model.Email>(){createdEmail});

            var request = new ReSendRequest()
            {
                ArchivedEmailTimeStamp = timeStamp
            };

            // Act
            var response = await _reSend.HandleAsync(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task HandleAsync_ReturnsNotFound_IfArchivedEmailTemplateHasNoUnSuccessfullSendingResult()
        {
            // Arrange
            var timeStamp = DateTime.Now;

            _archiveEmailTemplateRepositoryMock.Setup(repository => repository.GetAll())
                .Returns(new List<Core.Model.ArchiveEmailTemplate>()
                {
                    new Core.Model.ArchiveEmailTemplate
                    {
                        TimeStamp = timeStamp,
                        SendingResults = new SendingResult[]
                        {
                            new SendingResult(TestEmailAddress, true)
                        }
                    }
                });


            var request = new ReSendRequest()
            {
                ArchivedEmailTimeStamp = timeStamp
            };

            // Act
            var response = await _reSend.HandleAsync(request);

            // Assert
            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async Task HandleAsync_ReturnsOk_IfEmailsReSent()
        {
            // Arrange
            var timeStamp = DateTime.Now;

            _archiveEmailTemplateRepositoryMock.Setup(repository => repository.GetAll())
                .Returns(new List<Core.Model.ArchiveEmailTemplate>()
                {
                    new Core.Model.ArchiveEmailTemplate
                    {
                        TimeStamp = timeStamp,
                        SendingResults = new SendingResult[]
                        {
                            new SendingResult(TestEmailAddress, false)
                        }
                    }
                });

            var createdEmail = new Core.Model.Email
            {
                ToAddress = TestEmailAddress
            };
            _emailCreatorMock.Setup(emailCreator => emailCreator.CreateEmailsAsync(It.IsAny<EmailTemplate>()))
                .ReturnsAsync(new List<Core.Model.Email>() { createdEmail });

            _emailSenderMock.Setup(sender => sender.SendAsync(It.IsAny<IEnumerable<Core.Model.Email>>()))
                .ReturnsAsync(new List<SendingResult>()
                {
                    new SendingResult(TestEmailAddress, true)
                });

            var request = new ReSendRequest()
            {
                ArchivedEmailTimeStamp = timeStamp
            };

            // Act
            var response = await _reSend.HandleAsync(request);

            // Assert
            Assert.IsType<OkObjectResult>(response.Result);

            var result = response.Result as OkObjectResult;
            Assert.IsAssignableFrom<IEnumerable<SendingResult>>(result.Value);
        }
    }
}
