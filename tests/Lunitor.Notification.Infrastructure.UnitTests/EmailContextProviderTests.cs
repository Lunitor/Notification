using Lunitor.Notification.Core;
using Lunitor.Notification.Core.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
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
        public void ConstructorThrowsArgumentNullExceptionWhenUserRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailContextProvider(null));
        }

        [Fact]
        public void GetEmailContextReturnsNotNullEmailContext()
        {
            var context = _emailContextProvider.GetEmailContext();

            Assert.NotNull(context);
        }

        [Fact]
        public void GetEmailContextReturnsEmailContextWithNotNullUsers()
        {
            var context = _emailContextProvider.GetEmailContext();

            Assert.NotNull(context.Users);
        }

        [Fact]
        public void GetEmailContextReturnsEmailContextWithEmptyUsersIfNoUsersReturnedFromUserRepository()
        {
            _userRepositoryMock.Setup(repository => repository.GetAll())
                .Returns(new List<User>());

            var context = _emailContextProvider.GetEmailContext();

            Assert.Empty(context.Users);
        }

        [Fact]
        public void GetEmailContextReturnsEmailContextWithUsersThatReturnedFromUserRepository()
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
