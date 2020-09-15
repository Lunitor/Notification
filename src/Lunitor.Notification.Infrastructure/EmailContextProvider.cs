using Ardalis.GuardClauses;
using Lunitor.Notification.Core;
using Lunitor.Notification.Core.Model;

namespace Lunitor.Notification.Infrastructure
{
    internal class EmailContextProvider : IEmailContextProvider
    {
        private readonly IUserRepository _userRepository;

        public EmailContextProvider(IUserRepository userRepository)
        {
            Guard.Against.Null(userRepository, nameof(userRepository));

            _userRepository = userRepository;
        }

        public EmailContext GetEmailContext()
        {
            var users = _userRepository.GetAll();

            var context = new EmailContext();
            context.Users.AddRange(users);

            return context;
        }
    }
}
