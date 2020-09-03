using Lunitor.Notification.Core.Factory;
using Lunitor.Notification.Core.Model;
using System.Linq;
using Xunit;

namespace Lunitor.Notification.Core.UnitTests.Factory
{
    public class CommonEmailFactoryTests
    {
        private  CommonEmailFactory _commonEmailFactory;

        private readonly EmailTemplateContent TestEmailTemplateContent;

        public CommonEmailFactoryTests()
        {
            _commonEmailFactory = new CommonEmailFactory();

            TestEmailTemplateContent = new EmailTemplateContent("test subject", "test text");
        }

        [Fact]
        public void CreateEmailsReturnsEmptyEnumerableOfEmailsWhenThereAreNoUsersInEmailContext()
        {
            var emailContext = new EmailContext();

            var emails = _commonEmailFactory.CreateEmails(TestEmailTemplateContent, emailContext);

            Assert.NotNull(emails);
        }

        [Fact]
        public void CreateEmailsReturnsOneEmailWithToAddressEmpty()
        {
            var emailContext = new EmailContext();
            emailContext.Users.Add(new User() { Name = "test user 1", EmailAddress = "user1@test.net" });
            emailContext.Users.Add(new User() { Name = "test user 2", EmailAddress = "user2@test.net" });

            var emails = _commonEmailFactory.CreateEmails(TestEmailTemplateContent, emailContext);

            Assert.True(string.IsNullOrEmpty(emails.First().ToAddress));
        }

        [Fact]
        public void CreateEmailsReturnsOneEmailWithMultipleBCCAddressesIfMultipleUsersAreInTheEmailContext()
        {
            var emailContext = new EmailContext();
            emailContext.Users.Add(new User() { Name = "test user 1", EmailAddress = "user1@test.net" });
            emailContext.Users.Add(new User() { Name = "test user 2", EmailAddress = "user2@test.net" });

            var emails = _commonEmailFactory.CreateEmails(TestEmailTemplateContent, emailContext);

            Assert.NotNull(emails);
            Assert.Single(emails);
            Assert.Equal(emailContext.Users.Count, emails.First().BCCAddresses.Count);
        }

        [Fact]
        public void CreateEmailsReturnsOneEmailWithBCCAddressesMatchingWithTheUsersEmailAddressesInTheEmailContext()
        {
            var emailContext = new EmailContext();
            emailContext.Users.Add(new User() { Name = "test user 1", EmailAddress = "user1@test.net" });
            emailContext.Users.Add(new User() { Name = "test user 2", EmailAddress = "user2@test.net" });

            var emails = _commonEmailFactory.CreateEmails(TestEmailTemplateContent, emailContext);

            for (int i = 0; i < emailContext.Users.Count; i++)
            {
                Assert.Equal(emailContext.Users[i].EmailAddress, emails.First().BCCAddresses[i]);
            }
        }
    }
}
