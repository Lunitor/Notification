using Lunitor.Notification.Infrastructure.Repository;
using Xunit;

namespace Lunitor.Notification.Infrastructure.UnitTests.Repository
{
    public class JellyfinUserDtoTests
    {
        [Fact]
        public void Map_ReturnsUserWithAllItsPropertiesMappedWithDtosProperties()
        {
            var dto = new JellyfinUserDto
            {
                UserId = "01134fggt6676rtgef2342rewfew",
                Username = "test user",
                EmailAddress = "test.user@test.net"
            };

            var model = dto.Map();

            Assert.Equal(dto.Username, model.Name);
            Assert.Equal(dto.EmailAddress, model.EmailAddress);
        }
    }
}
