using Lunitor.Notification.Infrastructure.Repository;
using System;
using Xunit;

namespace Lunitor.Notification.Infrastructure.UnitTests.Repository
{
    public class LiteDbContextTests
    {
        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfLiteDbConfigurationOptionsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new LiteDbContext(null));
        }
    }
}
