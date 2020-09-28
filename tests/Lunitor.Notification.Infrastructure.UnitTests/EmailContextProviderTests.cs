using Lunitor.Notification.Core.Model;
using Lunitor.Notification.Core.Repository;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Lunitor.Notification.Infrastructure.UnitTests
{
    public class EmailContextProviderTests
    {
        private EmailContextProvider _emailContextProvider;

        private Mock<IUserRepository> _userRepositoryMock;

        public EmailContextProviderTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();

            _emailContextProvider = new EmailContextProvider(_userRepositoryMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenUserRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailContextProvider(null));
        }

        [Fact]
        public void GetEmailContext_ReturnsNotNullEmailContext()
        {
            var context = _emailContextProvider.GetEmailContext();

            Assert.NotNull(context);
        }

        [Fact]
        public void GetEmailContext_ReturnsEmailContext_WithNotNullUsers()
        {
            var context = _emailContextProvider.GetEmailContext();

            Assert.NotNull(context.Users);
        }

        [Fact]
        public void GetEmailContext_ReturnsEmailContext_WithEmptyUsers_WhenNoUsersReturnedFromUserRepository()
        {
            _userRepositoryMock.Setup(repository => repository.GetAll())
                .Returns(new List<User>());

            var context = _emailContextProvider.GetEmailContext();

            Assert.Empty(context.Users);
        }

        [Fact]
        public void GetEmailContext_ReturnsEmailContext_WithUsersThatReturnedFromUserRepository()
        {
            var testUsers = new List<User>
                {
                    new User()
                    {
                        Name = "Test1",
                        EmailAddress = "one@test.net"
                    },
                    new User()
                    {
                        Name = "Test2",
                        EmailAddress = "two@test.net"
                    },
                };

            _userRepositoryMock.Setup(repository => repository.GetAll())
                .Returns(testUsers);

            var context = _emailContextProvider.GetEmailContext();

            Assert.NotEmpty(context.Users);
            Assert.Equal(testUsers, context.Users);
        }
    }
}
