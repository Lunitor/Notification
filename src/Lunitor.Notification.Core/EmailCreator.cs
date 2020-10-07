using Ardalis.GuardClauses;
using Lunitor.Notification.Core.Factory;
using Lunitor.Notification.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lunitor.Notification.Core
{
    internal class EmailCreator : IEmailCreator
    {
        private readonly IEmailContextProvider _emailContextProvider;
        private readonly IEmailFactoryProducer _emailFactoryProducer;

        public EmailCreator(IEmailContextProvider emailContextProvider, IEmailFactoryProducer emailFactoryProducer)
        {
            Guard.Against.Null(emailContextProvider, nameof(emailContextProvider));
            Guard.Against.Null(emailFactoryProducer, nameof(emailFactoryProducer));

            _emailContextProvider = emailContextProvider;
            _emailFactoryProducer = emailFactoryProducer;
        }

        public async Task<IEnumerable<Email>> CreateEmailsAsync(EmailTemplate template)
        {
            Guard.Against.Null(template, nameof(template));
            Guard.Against.NullOrEmpty(template.Type, nameof(template.Type));
            Guard.Against.NullOrEmpty(template.Content.Subject, nameof(template.Content.Subject));
            Guard.Against.NullOrEmpty(template.Content.Text, nameof(template.Content.Text));

            var emailContext = await _emailContextProvider.GetEmailContextAsync();

            EmailFactory emailFactory = _emailFactoryProducer.GetEmailFactory(template.Type);

            return emailFactory.CreateEmails(template.Content, emailContext);
        }
    }
}
