using Lunitor.Notification.Core.Model;
using Lunitor.Notification.Infrastructure.Configuration;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Lunitor.Notification.Infrastructure.UnitTests
{
    public class EmailSenderTests
    {
        private EmailSender _emailSender;

        private Mock<IOptions<SmtpConfiguration>> _smtpConfigurationMock;
        private Mock<ISmtpClient> _smtpClientMock;
        private Mock<ILogger<EmailSender>> _loggerMock;

        private readonly SmtpConfiguration TestSmtpConfiguration = new SmtpConfiguration
        {
            SmtpServer = "test.server.net",
            SmtpPort = 587,
            SenderName = "Test",
            SenderEmail = "test@test.net",
            SenderUserName = "testuser",
            SenderPassword = "password"
        };

        private readonly List<Email> TestSingleRecepientEmails = new List<Email>
            {
                new Email
                {
                    ToAddress = "user1@test.net",
                    Subject = "test subject",
                    Body = "test text"
                },
                new Email
                {
                    ToAddress = "user2@test.net",
                    Subject = "test subject",
                    Body = "test text"
                },
                new Email
                {
                    ToAddress = "user3@test.net",
                    Subject = "test subject",
                    Body = "test text"
                },
                new Email
                {
                    ToAddress = "user4@test.net",
                    Subject = "test subject",
                    Body = "test text"
                },
            };

        public EmailSenderTests()
        {
            _smtpConfigurationMock = new Mock<IOptions<SmtpConfiguration>>();
            _smtpConfigurationMock.Setup(conf => conf.Value)
                .Returns(TestSmtpConfiguration);

            _smtpClientMock = new Mock<ISmtpClient>();

            _loggerMock = new Mock<ILogger<EmailSender>>();

            _emailSender = new EmailSender(_smtpConfigurationMock.Object, _smtpClientMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfSmtpConfigurationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailSender(null, _smtpClientMock.Object, _loggerMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfSmtpClientIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailSender(_smtpConfigurationMock.Object, null, _loggerMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailSender(_smtpConfigurationMock.Object, _smtpClientMock.Object, null));
        }

        [Fact]
        public async Task SendAsync_ThrowsArgumentNullException_IfEmailsIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _emailSender.SendAsync(null));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task SendAsync_CallsSmtpClientSendAsyncMethod_AsManyTimesAsEmailsAreInEmails(int count)
        {
            // Arrange
            _smtpClientMock.SetupGet(c => c.IsConnected)
                .Returns(true);

            var emails = TestSingleRecepientEmails.Take(count);

            // Act
            await _emailSender.SendAsync(emails);

            // Assert
            _smtpClientMock.Verify(client => client.SendAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<ITransferProgress>()),
                Times.Exactly(count));
        }

        [Fact]
        public async Task SendAsync_CallsSmtpClientMethodsInOrder()
        {
            // Arrange
            var sequence = new List<string>();

            _smtpClientMock.SetupGet(c => c.Capabilities)
                .Returns(SmtpCapabilities.Authentication);
            _smtpClientMock.SetupGet(c => c.IsConnected)
                .Returns(true);

            _smtpClientMock.Setup(client => client.ConnectAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<SecureSocketOptions>(),
                It.IsAny<CancellationToken>()))
                .Callback(() => sequence.Add("Connect"))
                .Returns(Task.CompletedTask);
            _smtpClientMock.Setup(client => client.AuthenticateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .Callback(() => sequence.Add("Auth"))
                .Returns(Task.CompletedTask);
            _smtpClientMock.Setup(client => client.SendAsync(
                It.IsAny<MimeMessage>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<ITransferProgress>()))
                .Callback(() => sequence.Add("Send"))
                .Returns(Task.CompletedTask);
            _smtpClientMock.Setup(client => client.DisconnectAsync(
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
                .Callback(() => sequence.Add("Disconnect"))
                .Returns(Task.CompletedTask);

            // Act
            await _emailSender.SendAsync(TestSingleRecepientEmails);

            // Assert
            Assert.Equal("Connect", sequence.First());
            Assert.Equal("Auth", sequence[1]);
            for (int i = 2; i < TestSingleRecepientEmails.Count; i++)
            {
                Assert.Equal("Send", sequence[i]);
            }
            Assert.Equal("Disconnect", sequence.Last());
        }

        [Fact]
        public async Task SendAsync_CallsSmtpClientDisconnet_IfErrorThrownDuringAuthentication()
        {
            // Arrange
            var sequence = new List<string>();

            var isConnected = false;

            _smtpClientMock.SetupGet(c => c.Capabilities)
                .Returns(SmtpCapabilities.Authentication);
            _smtpClientMock.SetupGet(c => c.IsConnected)
                .Returns(() => isConnected);

            _smtpClientMock.Setup(client => client.ConnectAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<SecureSocketOptions>(),
                It.IsAny<CancellationToken>()))
                .Callback(() =>
                    {
                        sequence.Add("Connect");
                        isConnected = true;
                    })
                .Returns(Task.CompletedTask);
            _smtpClientMock.Setup(client => client.AuthenticateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .Callback(() => sequence.Add("Auth"))
                .Returns(() =>
                    {
                        throw new Exception();
                    });
            _smtpClientMock.Setup(client => client.SendAsync(
                It.IsAny<MimeMessage>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<ITransferProgress>()))
                .Callback(() => sequence.Add("Send"))
                .Returns(Task.CompletedTask);
            _smtpClientMock.Setup(client => client.DisconnectAsync(
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
                .Callback(() =>
                    {
                        sequence.Add("Disconnect");
                        isConnected = false;
                    })
                .Returns(Task.CompletedTask);

            // Act
            await _emailSender.SendAsync(TestSingleRecepientEmails);

            // Assert
            Assert.Equal(3, sequence.Count);
            Assert.Equal("Connect", sequence[0]);
            Assert.Equal("Auth", sequence[1]);
            Assert.Equal("Disconnect", sequence[2]);
        }

        [Fact]
        public async Task SendAsync_CallsSmtpClientAuthenticateAsyncWithSmtpConfigurationMembers_IfSmtpServerSupportsAuthentication()
        {
            // Arrange
            _smtpClientMock.SetupGet(c => c.Capabilities)
                .Returns(SmtpCapabilities.Authentication);

            _smtpClientMock.Setup(client => client.SendAsync(
                It.IsAny<MimeMessage>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<ITransferProgress>()))
                .Returns(Task.CompletedTask);

            // Act
            await _emailSender.SendAsync(TestSingleRecepientEmails);

            // Assert
            _smtpClientMock.Verify(client => client.AuthenticateAsync(
                It.Is<string>(username => username == TestSmtpConfiguration.SenderUserName),
                It.Is<string>(password => password == TestSmtpConfiguration.SenderPassword),
                It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Fact]
        public async Task SendAsync_NotCallsSmtpClientAuthenticateAsync_IfSmtpServerNotSupportsAuthentication()
        {
            // Arrange
            _smtpClientMock.SetupGet(c => c.Capabilities)
                .Returns(SmtpCapabilities.None);

            // Act
            await _emailSender.SendAsync(TestSingleRecepientEmails);

            // Assert
            _smtpClientMock.Verify(client => client.AuthenticateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task SendAsync_CallsSmtpClientSendAsyncWithAllEmails_EvenIfAnyThrowsException()
        {
            // Arrange
            _smtpClientMock.SetupGet(c => c.IsConnected)
                .Returns(true);

            _smtpClientMock.Setup(client => client.SendAsync(
                It.IsAny<MimeMessage>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<ITransferProgress>()))
                .Returns((MimeMessage message, CancellationToken token, ITransferProgress progress) =>
                {
                    if (message.To.First().ToString() == TestSingleRecepientEmails.First().ToAddress)
                    {
                        throw new Exception();
                    }

                    return Task.CompletedTask;
                });

            // Act
            await _emailSender.SendAsync(TestSingleRecepientEmails);

            // Assert
            _smtpClientMock.Verify(client => client.SendAsync(
                It.Is<MimeMessage>(message =>
                    message.From.Mailboxes.First().Name == TestSmtpConfiguration.SenderName &&
                    message.From.Mailboxes.First().Address == TestSmtpConfiguration.SenderEmail),
                It.IsAny<CancellationToken>(),
                It.IsAny<ITransferProgress>()),
                Times.Exactly(TestSingleRecepientEmails.Count));
        }
    }
}
