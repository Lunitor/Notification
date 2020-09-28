using Lunitor.Notification.Core;
using Lunitor.Notification.Core.Model;
using Lunitor.Notification.Web.Endpoints.Email;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Lunitor.Notification.Web.UnitTests.Endpoints.Email
{
    public class SendTests
    {
        private Send _send;

        private Mock<IEmailCreator> _emailCreatorMock;
        private Mock<IEmailSender> _emailSenderMock;

        public SendTests()
        {
            _emailCreatorMock = new Mock<IEmailCreator>();
            _emailSenderMock = new Mock<IEmailSender>();

            _send = new Send(_emailCreatorMock.Object, _emailSenderMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullExcpetion_WhenEmailCreatorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Send(null, _emailSenderMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullExcpetion_WhenEmailSenderIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Send(_emailCreatorMock.Object, null));
        }

        [Fact]
        public async Task HandleAsync_ReturnsBadRequest_WhenModelValidationFails()
        {
            _send.ModelState.AddModelError("title", "title can't be empty");

            var response = await _send.HandleAsync(new SendEmailRequest());

            Assert.IsType<BadRequestObjectResult>(response.Result);

            var result = response.Result as BadRequestObjectResult;
            Assert.NotNull(result.Value);

            Assert.Null(response.Value);
        }

        [Fact]
        public async Task HandleAsync_ReturnsOk_WhenModelValidationSucceed()
        {
            var response = await _send.HandleAsync(new SendEmailRequest());

            Assert.IsType<OkObjectResult>(response.Result);

            var result = response.Result as OkObjectResult;
            Assert.NotNull(result.Value);

            Assert.Null(response.Value);
        }

        [Fact]
        public async Task HandleAsync_ReturnsOkWithResponseTypeEquelsToRequestType_WhenModelValidationSucceed()
        {
            var request = new SendEmailRequest
            {
                Type = "byuser"
            };

            var generatedEmails = new List<Core.Model.Email>
            {
                new Core.Model.Email(),
                new Core.Model.Email()
            };
            _emailCreatorMock.Setup(ec => ec.CreateEmails(It.IsAny<EmailTemplate>()))
                .Returns(generatedEmails);

            var response = await _send.HandleAsync(request);

            Assert.IsType<OkObjectResult>(response.Result);
            var result = response.Result as OkObjectResult;
            var value = result.Value as SendEmailResponse;
            Assert.Equal(request.Type, value.Type);
            Assert.Equal(generatedEmails.Count, value.SentEmailCount);
        }

        [Fact]
        public async Task HandleAsync_CallsEmailSenderSendAsync_WhenModelValidationSucceed()
        {
            var response = await _send.HandleAsync(new SendEmailRequest());

            _emailSenderMock.Verify(es => es.SendAsync(It.IsAny<IEnumerable<Core.Model.Email>>()), Times.Once);
        }
    }
}
