using Lunitor.Notification.Core.Model;
using Lunitor.Notification.Infrastructure.Configuration;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
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

        private readonly SmtpConfiguration TestSmtpConfiguration = new SmtpConfiguration
        {
            SmtpServer = "test.server.net",
            SmtpPort = 587,
            SenderName = "Test",
            SenderEmail = "test@test.net",
            SenderUserName = "testuser",
            SenderPassword = "password"
        };

        private readonly List<Email> TestEmails = new List<Email>
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

            _emailSender = new EmailSender(_smtpConfigurationMock.Object, _smtpClientMock.Object);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenSmtpConfigurationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailSender(null, _smtpClientMock.Object));
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenSmtpClientIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailSender(_smtpConfigurationMock.Object, null));
        }

        [Fact]
        public async Task SendAsyncThrowsArgumentNullExceptionWhenEmailsIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _emailSender.SendAsync(null));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task SendAsyncCallsSmtpClientSendAsyncMethodAsManyTimesAsEmailsAreInEmails(int count)
        {
            var emails = TestEmails.Take(count);

            await _emailSender.SendAsync(emails);

            _smtpClientMock.Verify(client => client.SendAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<ITransferProgress>()),
                Times.Exactly(count));
        }

        [Fact]
        public async Task SendAsyncCallsSmtpClientMethodsInOrder()
        {
            var sequence = new List<string>();

            var smtpClientMock = new Mock<ISmtpClient>(MockBehavior.Strict);
            smtpClientMock.Setup(client => client.ConnectAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<SecureSocketOptions>(),
                It.IsAny<CancellationToken>()))
                .Callback(() => sequence.Add("Connect"))
                .Returns(Task.CompletedTask);
            smtpClientMock.Setup(client => client.AuthenticateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .Callback(() => sequence.Add("Auth"))
                .Returns(Task.CompletedTask);
            smtpClientMock.Setup(client => client.SendAsync(
                It.IsAny<MimeMessage>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<ITransferProgress>()))
                .Callback(() => sequence.Add("Send"))
                .Returns(Task.CompletedTask);
            smtpClientMock.Setup(client => client.DisconnectAsync(
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
                .Callback(() => sequence.Add("Disconnect"))
                .Returns(Task.CompletedTask);


            var emailSender = new EmailSender(_smtpConfigurationMock.Object, smtpClientMock.Object);
            await emailSender.SendAsync(TestEmails);

            Assert.Equal("Connect", sequence.First());
            Assert.Equal("Auth", sequence[1]);
            for (int i = 2; i < TestEmails.Count; i++)
            {
                Assert.Equal("Send", sequence[i]);
            }
            Assert.Equal("Disconnect", sequence.Last());
        }

        [Fact]
        public async Task SendAsyncCallsSmtpClientMethodsWithSmtpConfigurationMemebers()
        {
            await _emailSender.SendAsync(TestEmails);

            _smtpClientMock.Verify(client => client.SendAsync(
                It.Is<MimeMessage>(message =>
                    message.From.Mailboxes.First().Name == TestSmtpConfiguration.SenderName &&
                    message.From.Mailboxes.First().Address == TestSmtpConfiguration.SenderEmail),
                It.IsAny<CancellationToken>(),
                It.IsAny<ITransferProgress>()),
                Times.Exactly(TestEmails.Count));

            _smtpClientMock.Verify(client => client.ConnectAsync(
                It.Is<string>(host => host == TestSmtpConfiguration.SmtpServer),
                It.Is<int>(port => port == TestSmtpConfiguration.SmtpPort),
                It.IsAny<SecureSocketOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Once());

            _smtpClientMock.Verify(client => client.AuthenticateAsync(
                It.Is<string>(username => username == TestSmtpConfiguration.SenderUserName),
                It.Is<string>(password => password == TestSmtpConfiguration.SenderPassword),
                It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}
