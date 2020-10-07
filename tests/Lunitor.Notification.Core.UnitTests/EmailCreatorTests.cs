using Lunitor.Notification.Core.Factory;
using Lunitor.Notification.Core.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Lunitor.Notification.Core.UnitTests
{
    public class EmailCreatorTests
    {
        private readonly EmailCreator _emailCreator;

        private Mock<IEmailContextProvider> _emailContextProviderMock;
        private Mock<IEmailFactoryProducer> _emailFactoryProducerMock;
        private Mock<ByUserEmailFactory> _byUserEmailFactoryMock;
        private Mock<CommonEmailFactory> _commonEmailFactoryMock;

        public EmailCreatorTests()
        {
            _emailContextProviderMock = new Mock<IEmailContextProvider>();

            _emailFactoryProducerMock = new Mock<IEmailFactoryProducer>();
            _emailFactoryProducerMock.Setup(producer => producer.GetEmailFactory(It.IsAny<string>()))
                .Returns(( string templateType) =>
                {
                    if (templateType.ToUpperInvariant() == "BYUSER")
                        return _byUserEmailFactoryMock.Object;

                    return _commonEmailFactoryMock.Object;
                });

            _byUserEmailFactoryMock = new Mock<ByUserEmailFactory>();
            _byUserEmailFactoryMock.Setup(byuserFactory => byuserFactory.CreateEmails(It.IsAny<EmailTemplateContent>(), It.IsAny<EmailContext>()))
                .Returns(new List<Email>());

            _commonEmailFactoryMock = new Mock<CommonEmailFactory>();
            _commonEmailFactoryMock.Setup(commonFactory => commonFactory.CreateEmails(It.IsAny<EmailTemplateContent>(), It.IsAny<EmailContext>()))
                .Returns(new List<Email>());

            _emailCreator = new EmailCreator(_emailContextProviderMock.Object, _emailFactoryProducerMock.Object);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenEmailContextProviderIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailCreator(null, _emailFactoryProducerMock.Object));
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionWhenEmailFactoryProducerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailCreator(_emailContextProviderMock.Object, null));
        }

        [Fact]
        public async Task CreateEmailsAsyncThrowsArgumentNullExceptionWhenTemplateIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _emailCreator.CreateEmailsAsync(null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task CreateEmailsAsyncThrowsArgumentExceptionWhenTemplateTextIsNullOrEmpty(string text)
        {
            var template = new EmailTemplate("testtype", "test subject", text);

            await Assert.ThrowsAnyAsync<ArgumentException>(() => _emailCreator.CreateEmailsAsync(template));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task CreateEmailsAsyncThrowsArgumentExceptionWhenTemplateTypeIsNullOrEmpty(string type)
        {
            var template = new EmailTemplate(type, "test subject", "test text");

            await Assert.ThrowsAnyAsync<ArgumentException>(() => _emailCreator.CreateEmailsAsync(template));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task CreateEmailsAsyncThrowsArgumentExceptionWhenTemplateSubjectIsNullOrEmpty(string subject)
        {
            var template = new EmailTemplate("test type", subject, "test text");

            await Assert.ThrowsAnyAsync<ArgumentException>(() => _emailCreator.CreateEmailsAsync(template));
        }
    }
}
