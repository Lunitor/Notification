using Lunitor.Notification.Core.Factory;
using System;
using Xunit;

namespace Lunitor.Notification.Core.UnitTests.Factory
{
    public class EmailFactoryProducerTests
    {
        private EmailFactoryProducer _emailFactoryProducer;

        public EmailFactoryProducerTests()
        {
            _emailFactoryProducer = new EmailFactoryProducer();
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
