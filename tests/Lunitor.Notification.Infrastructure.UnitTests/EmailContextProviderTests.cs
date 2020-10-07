using Lunitor.Notification.Core.Model;
using Lunitor.Notification.Core.Repository;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task GetEmailContextAsync_ReturnsNotNullEmailContext()
        {
            var context = await _emailContextProvider.GetEmailContextAsync();

            Assert.NotNull(context);
        }

        [Fact]
        public async Task GetEmailContextAsync_ReturnsEmailContext_WithNotNullUsers()
        {
            var context = await _emailContextProvider.GetEmailContextAsync();

            Assert.NotNull(context.Users);
        }

        [Fact]
        public async Task GetEmailContextAsync_ReturnsEmailContext_WithEmptyUsers_WhenNoUsersReturnedFromUserRepository()
        {
            _userRepositoryMock.Setup(repository => repository.GetAllAsync())
                .Returns(Task.FromResult<IEnumerable<User>>(new List<User>()));

            var context = await _emailContextProvider.GetEmailContextAsync();

            Assert.Empty(context.Users);
        }

        [Fact]
        public async Task GetEmailContextAsync_ReturnsEmailContext_WithUsersThatReturnedFromUserRepository()
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

            _userRepositoryMock.Setup(repository => repository.GetAllAsync())
                .Returns(Task.FromResult<IEnumerable<User>>(testUsers));

            var context = await _emailContextProvider.GetEmailContextAsync();

            Assert.NotEmpty(context.Users);
            Assert.Equal(testUsers, context.Users);
        }
    }
}
