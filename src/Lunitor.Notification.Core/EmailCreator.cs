using Ardalis.GuardClauses;
using Lunitor.Notification.Core.Factory;
using Lunitor.Notification.Core.Model;
using System.Collections.Generic;

namespace Lunitor.Notification.Core
{
    class EmailCreator
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

        public IEnumerable<Email> CreateEmails(EmailTemplate template)
        {
            Guard.Against.Null(template, nameof(template));
            Guard.Against.NullOrEmpty(template.Type, nameof(template.Type));
            Guard.Against.NullOrEmpty(template.Content.Subject, nameof(template.Content.Subject));
            Guard.Against.NullOrEmpty(template.Content.Text, nameof(template.Content.Text));

            var emailContext = _emailContextProvider.GetEmailContext();

            EmailFactory emailFactory = _emailFactoryProducer.GetEmailFactory(template.Type);

            return emailFactory.CreateEmails(template.Content, emailContext);
        }
    }
}
