using Lunitor.Notification.Core.Factory;
using Lunitor.Notification.Core.Model;
using System.Linq;
using Xunit;

namespace Lunitor.Notification.Core.UnitTests.Factory
{
    public class ByUserEmailFactoryTests
    {
        private ByUserEmailFactory _byUserEmailFactory;

        private readonly EmailContext TestEmailContext;
        private readonly EmailTemplateContent TestEmailTemplateContent;

        public ByUserEmailFactoryTests()
        {
            _byUserEmailFactory = new ByUserEmailFactory();

            TestEmailContext = new EmailContext();
            TestEmailContext.Users.Add(new User() { Name = "test user 1", EmailAddress = "user1@test.net" });
            TestEmailContext.Users.Add(new User() { Name = "test user 2", EmailAddress = "user2@test.net" });
            TestEmailContext.Users.Add(new User() { Name = "test user 3", EmailAddress = "user3@test.net" });
            TestEmailContext.Users.Add(new User() { Name = "test user 4", EmailAddress = "user4@test.net" });

            TestEmailTemplateContent = new EmailTemplateContent("test subject", "test text");
        }

        [Fact]
        public void CreateEmailsReturnsEmptyEnumerableOfEmailsWhenThereAreNoUsersInEmailContext()
        {
            var emailContext = new EmailContext();

            var emails = _byUserEmailFactory.CreateEmails(TestEmailTemplateContent, emailContext);

            Assert.NotNull(emails);
        }

        [Fact]
        public void CreateEmailsReturnsMultipleEmailsIfMultipleUsersAreInTheEmailContext()
        {
            var emails = _byUserEmailFactory.CreateEmails(TestEmailTemplateContent, TestEmailContext);

            Assert.NotNull(emails);
            Assert.Equal(TestEmailContext.Users.Count, emails.Count());
        }

        [Fact]
        public void CreateEmailsPutTemplateSubjectToEmailSubject()
        {
            var emails = _byUserEmailFactory.CreateEmails(TestEmailTemplateContent, TestEmailContext);

            Assert.NotNull(emails);
            foreach (var email in emails)
            {
                Assert.Equal(TestEmailTemplateContent.Subject, email.Subject);
            }
        }

        [Fact]
        public void CreateEmailsReturnsEmailsForAllTheUsersInContextWithTheUsersEmailAddressesInToAddress()
        {
            var emails = _byUserEmailFactory.CreateEmails(TestEmailTemplateContent, TestEmailContext).ToList();

            for (int i = 0; i < TestEmailContext.Users.Count; i++)
            {
                Assert.Equal(TestEmailContext.Users[i].EmailAddress, emails[i].ToAddress);
            }
        }

        [Fact]
        public void CreateEmailsReturnsEmailsWithEmptyBCCAddresses()
        {
            var emails = _byUserEmailFactory.CreateEmails(TestEmailTemplateContent, TestEmailContext).ToList();

            foreach (var email in emails)
            {
                Assert.Empty(email.BCCAddresses);
            }
        }

        [Fact]
        public void CreateEmailReturnsEmailsWithNotEmptyBodyAndUserNamePlaceholdersAreReplacedByActualUserNames()
        {
            var userNamePlaceholder = _byUserEmailFactory.Placeholders["UserName"];
            var templateContent = new EmailTemplateContent("test subject", $"Hello {userNamePlaceholder}");

            var emails = _byUserEmailFactory.CreateEmails(templateContent, TestEmailContext).ToList();

            for (int i = 0; i < TestEmailContext.Users.Count; i++)
            {
                Assert.DoesNotContain(userNamePlaceholder, emails[i].Body);
                Assert.Contains(TestEmailContext.Users[i].Name, emails[i].Body);
            }
        }

        [Fact]
        public void CreateEmailReturnsEmailsWithNotEmptyBodyAndUnknownPlaceholdersAreNotReplaced()
        {
            var unknownPlaceholder = "{PLACEHOLDER}";
            var templateContent = new EmailTemplateContent("test subject", $"Hello {unknownPlaceholder}");

            var emails = _byUserEmailFactory.CreateEmails(templateContent, TestEmailContext).ToList();

            for (int i = 0; i < TestEmailContext.Users.Count; i++)
            {
                Assert.Contains(unknownPlaceholder, emails[i].Body);
            }
        }
    }
}
