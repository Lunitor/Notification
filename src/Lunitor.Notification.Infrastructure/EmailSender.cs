using Ardalis.GuardClauses;
using Lunitor.Notification.Core;
using Lunitor.Notification.Core.Model;
using Lunitor.Notification.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lunitor.Notification.Infrastructure
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpConfiguration _smtpConfiguration;
        private readonly ISmtpClient _smtpClient;

        public EmailSender(IOptions<SmtpConfiguration> smtpConfiguration, ISmtpClient smtpClient)
        {
            Guard.Against.Null(smtpConfiguration, nameof(smtpConfiguration));
            Guard.Against.Null(smtpClient, nameof(smtpClient));

            _smtpConfiguration = smtpConfiguration.Value;
            _smtpClient = smtpClient;
        }

        public async Task SendAsync(IEnumerable<Email> emails)
        {
            Guard.Against.Null(emails, nameof(emails));

            await _smtpClient.ConnectAsync(_smtpConfiguration.SmtpServer, _smtpConfiguration.SmtpPort);
            await _smtpClient.AuthenticateAsync(_smtpConfiguration.SenderUserName, _smtpConfiguration.SenderPassword);

            List<Task> emailSendingTasks = GenerateEmailSendingTasks(emails);
            await Task.WhenAll(emailSendingTasks);

            await _smtpClient.DisconnectAsync(true);
        }

        private List<Task> GenerateEmailSendingTasks(IEnumerable<Email> emails)
        {
            var emailsendingTasks = new List<Task>();
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

                emailsendingTasks.Add(_smtpClient.SendAsync(message));
            }

            return emailsendingTasks;
        }
    }
}
