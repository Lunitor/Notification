using Lunitor.Notification.Core.Model;

namespace Lunitor.Notification.Infrastructure.Repository
{
    internal class JellyfinUserDto
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }

        public User Map()
        {
            return new User
            {
                Name = Username,
                EmailAddress = EmailAddress
            };
        }
    }
}
