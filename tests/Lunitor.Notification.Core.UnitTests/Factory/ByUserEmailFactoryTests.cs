using Lunitor.Notification.Core.Factory;
using Lunitor.Notification.Core.Model;
using System.Linq;
using Xunit;

namespace Lunitor.Notification.Core.UnitTests.Factory
{
    public class ByUserEmailFactoryTests
    {
        private  ByUserEmailFactory _byUserEmailFactory;

        public ByUserEmailFactoryTests()
        {
            _byUserEmailFactory = new ByUserEmailFactory();
        }

        [Fact]
        public void CreateEmailsReturnsEmptyEnumerableOfEmailsWhenThereAreNoUsersInEmailContext()
        {
            var emailContext = new EmailContext();

            var template = new EmailTemplate
            {
                Text = "test text",
                Type = "test type",
                Subject = "test subject"
            };

            var emails = _byUserEmailFactory.CreateEmails(template, emailContext);

            Assert.NotNull(emails);
        }

        [Fact]
        public void CreateEmailsReturnsMultipleEmailsIfMultipleUsersAreInTheEmailContext()
        {
            var emailContext = new EmailContext();
            emailContext.Users.Add(new User() { Name = "test user 1", EmailAddress = "user1@test.net" });
            emailContext.Users.Add(new User() { Name = "test user 2", EmailAddress = "user2@test.net" });

            var template = new EmailTemplate
            {
                Text = "test text",
                Type = "ByUser",
                Subject = "test subject"
            };

            var emails = _byUserEmailFactory.CreateEmails(template, emailContext);

            Assert.NotNull(emails);
            Assert.Equal(emailContext.Users.Count, emails.Count());
        }
    }
}
