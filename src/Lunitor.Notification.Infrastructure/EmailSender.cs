using Ardalis.GuardClauses;
using Lunitor.Notification.Core;
using Lunitor.Notification.Core.Model;
using Lunitor.Notification.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lunitor.Notification.Infrastructure
{
    internal class EmailSender : IEmailSender
    {
        private readonly SmtpConfiguration _smtpConfiguration;
        private readonly ISmtpClient _smtpClient;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<SmtpConfiguration> smtpConfiguration, ISmtpClient smtpClient, ILogger<EmailSender> logger)
        {
            Guard.Against.Null(smtpConfiguration, nameof(smtpConfiguration));
            Guard.Against.Null(smtpClient, nameof(smtpClient));
            Guard.Against.Null(logger, nameof(logger));

            _smtpConfiguration = smtpConfiguration.Value;
            _smtpClient = smtpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<SendingResult>> SendAsync(IEnumerable<Email> emails)
        {
            Guard.Against.Null(emails, nameof(emails));

            var sendingResults = new List<SendingResult>();

            await ConnectToSmtpServer();

            if (!_smtpClient.IsConnected)
            {
                return sendingResults;
            }

            var mimeMessages = GenerateMimeMessages(emails);
            foreach (var mimeMessage in mimeMessages)
            {
                try
                {
                    await _smtpClient.SendAsync(mimeMessage);

                    HandleSuccessfullTaskCompletion(mimeMessage, sendingResults);
                }
                catch (Exception ex)
                {
                    HandleFaultedTaskCompletion(ex, mimeMessage, sendingResults);
                }
            }

            await DisconnectFromSmtpServer();

            return sendingResults;
        }

        private async Task ConnectToSmtpServer()
        {
            try
            {
                await _smtpClient.ConnectAsync(_smtpConfiguration.SmtpServer, _smtpConfiguration.SmtpPort);

                if (_smtpClient.Capabilities.HasFlag(SmtpCapabilities.Authentication))
                {
                    await _smtpClient.AuthenticateAsync(_smtpConfiguration.SenderUserName, _smtpConfiguration.SenderPassword);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect/authenticate to the SMTP server ({address}:{port})!",
                    _smtpConfiguration.SmtpServer,
                    _smtpConfiguration.SmtpPort);

                if (_smtpClient.IsConnected)
                {
                    await _smtpClient.DisconnectAsync(true);
                }
            }
        }

        private async Task DisconnectFromSmtpServer()
        {
            try
            {
                await _smtpClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to disconnect from SMTP server!");
            }
        }

        private List<MimeMessage> GenerateMimeMessages(IEnumerable<Email> emails)
        {
            var mimeMessages = new List<MimeMessage>();
            foreach (var email in emails)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_smtpConfiguration.SenderName, _smtpConfiguration.SenderEmail));

                if (!string.IsNullOrEmpty(email.ToAddress))
                    message.To.Add(MailboxAddress.Parse(email.ToAddress));
                if (email.BCCAddresses.Count != 0)
                    message.Bcc.AddRange(email.BCCAddresses.Select(bcc => MailboxAddress.Parse(bcc)));

                message.Subject = email.Subject;
                message.Body = new TextPart(TextFormat.Plain)
                {
                    Text = email.Body
                };

                mimeMessages.Add(message);
            }

            return mimeMessages;
        }

        private void HandleSuccessfullTaskCompletion(MimeMessage mimeMessage, List<SendingResult> sendingResults)
        {
            if (mimeMessage.To.Count > 0)
            {
                _logger.LogInformation("Email [{subject}] sent to {emailAddress}!", mimeMessage.Subject, mimeMessage.To.First());
            }
            else
            {
                _logger.LogInformation("Email [{subject}] sent to multiple addresses in BCC!", mimeMessage.Subject);
            }

            sendingResults.Add(new SendingResult(mimeMessage.To.First().ToString(), true));
        }

        private void HandleFaultedTaskCompletion(Exception exception, MimeMessage mimeMessage, List<SendingResult> sendingResults)
        {
            if (mimeMessage.To.Count > 0)
            {
                _logger.LogError(exception, "Failed to send email [{subject}] to {emailAddress}!", mimeMessage.Subject, mimeMessage.To.First());
            }
            else
            {
                _logger.LogError(exception, "Failed to send email [{subject}] with multiple addresses in BCC!", mimeMessage.Subject);
            }

            sendingResults.Add(new SendingResult(mimeMessage.To.First().ToString(), false));
        }
    }
}
