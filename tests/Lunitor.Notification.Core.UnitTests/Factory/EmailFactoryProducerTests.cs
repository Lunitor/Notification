using Lunitor.Notification.Core.Factory;
using Lunitor.Notification.Core.Repository;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Lunitor.Notification.Core.UnitTests.Factory
{
    public class EmailFactoryProducerTests
    {
        private EmailFactoryProducer _emailFactoryProducer;

        private Mock<IEmailFactoryTypeRepository> _emailFactoryTypeRepositoryMock;

        public EmailFactoryProducerTests()
        {
            _emailFactoryTypeRepositoryMock = new Mock<IEmailFactoryTypeRepository>();
            _emailFactoryTypeRepositoryMock.Setup(repository => repository.GetAllTypes())
                .Returns(new List<Type>
                {
                    typeof(ByUserEmailFactory),
                    typeof(CommonEmailFactory)
                });

            _emailFactoryProducer = new EmailFactoryProducer(_emailFactoryTypeRepositoryMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentException_WhenEmailFactoryTypeRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailFactoryProducer(null));
        }

        [Theory]
        [InlineData("not known type")]
        [InlineData("")]
        public void GetEmailFactoryThrowsArgumentOutOfRangeExceptionIfTemplateTypeIsNotInTheKnownTypes(string templateType)
        {
            Assert.ThrowsAny<ArgumentOutOfRangeException>(() => _emailFactoryProducer.GetEmailFactory(templateType));
        }

        [Theory]
        [InlineData("ByUser")]
        [InlineData("byuser")]
        [InlineData("BYUSER")]
        public void GetEmailFactoryReturnsByUserEmailFactoryWhenTemplateTypeisByUser(string templateType)
        {
            var emailFactory = _emailFactoryProducer.GetEmailFactory(templateType);

            Assert.IsType<ByUserEmailFactory>(emailFactory);
        }

        [Theory]
        [InlineData("Common")]
        [InlineData("common")]
        [InlineData("COMMON")]
        public void GetEmailFactoryReturnsCommonEmailFactoryWhenTemplateTypeIsCommon(string templateType)
        {
            var emailFactory = _emailFactoryProducer.GetEmailFactory(templateType);

            Assert.IsType<CommonEmailFactory>(emailFactory);
        }
    }
}
