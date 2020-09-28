using Ardalis.GuardClauses;
using FluentValidation;
using Lunitor.Notification.Core.Repository;
using System.Linq;

namespace Lunitor.Notification.Web.Endpoints.Email
{
    public class SendEmailRequestValidator : AbstractValidator<SendEmailRequest>
    {
        private readonly IEmailFactoryTypeRepository _emailFactoryTypeRepository;

        public SendEmailRequestValidator(IEmailFactoryTypeRepository emailFactoryTypeRepository)
        {
            Guard.Against.Null(emailFactoryTypeRepository, nameof(emailFactoryTypeRepository));

            _emailFactoryTypeRepository = emailFactoryTypeRepository;

            RuleFor(x => x.Type)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(HaveAnEmailFactoryTypeEquivalent);
            RuleFor(x => x.Subject).NotEmpty();
            RuleFor(x => x.Body).NotEmpty();
        }

        private bool HaveAnEmailFactoryTypeEquivalent(string type)
        {
            var emailFactoryTypeName = _emailFactoryTypeRepository.GetAllNames()
                .FirstOrDefault(n => n.ToUpperInvariant() == type.ToUpperInvariant());

            return emailFactoryTypeName != null;
        }
    }
}
