using Lunitor.Notification.Core.Factory;
using Lunitor.Notification.Core.Model;
using System.Linq;
using Xunit;

namespace Lunitor.Notification.Core.UnitTests.Factory
{
    public class CommonEmailFactoryTests
    {
        private  CommonEmailFactory _commonEmailFactory;

        public CommonEmailFactoryTests()
        {
            _commonEmailFactory = new CommonEmailFactory();
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

            var emails = _commonEmailFactory.CreateEmails(template, emailContext);

            Assert.NotNull(emails);
        }

        [Fact]
        public void CreateEmailsReturnsOneEmailsWithMultipleBCCAddressesIfMultipleUsersAreInTheEmailContext()
        {
            var emailContext = new EmailContext();
            emailContext.Users.Add(new User() { Name = "test user 1", EmailAddress = "user1@test.net" });
            emailContext.Users.Add(new User() { Name = "test user 2", EmailAddress = "user2@test.net" });

            var template = new EmailTemplate
            {
                Text = "test text",
                Type = "Common",
                Subject = "test subject"
            };

            var emails = _commonEmailFactory.CreateEmails(template, emailContext);

            Assert.NotNull(emails);
            Assert.Single(emails);
            Assert.Equal(emailContext.Users.Count, emails.First().BCCAddresses.Count);
        }
    }
}
