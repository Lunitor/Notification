using Lunitor.Notification.Core.Factory;
using Lunitor.Notification.Core.Model;
using Moq;
using System;
using System.Collections.Generic;
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
            _byUserEmailFactoryMock.Setup(byuserFactory => byuserFactory.CreateEmails(It.IsAny<EmailTemplate>(), It.IsAny<EmailContext>()))
                .Returns(new List<Email>());

            _commonEmailFactoryMock = new Mock<CommonEmailFactory>();
            _commonEmailFactoryMock.Setup(commonFactory => commonFactory.CreateEmails(It.IsAny<EmailTemplate>(), It.IsAny<EmailContext>()))
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
        public void CreateEmailsThrowsArgumentNullExceptionWhenTemplateIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _emailCreator.CreateEmails(null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void CreateEmailsThrowsArgumentExceptionWhenTemplateTextIsNullOrEmpty(string text)
        {
            var template = new EmailTemplate
            {
                Text = text,
                Type = "testtype",
                Subject = "test subject"
            };

            Assert.ThrowsAny<ArgumentException>(() => _emailCreator.CreateEmails(template));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void CreateEmailsThrowsArgumentExceptionWhenTemplateTypeIsNullOrEmpty(string type)
        {
            var template = new EmailTemplate
            {
                Text = "test text",
                Type = type,
                Subject = "test subject"
            };

            Assert.ThrowsAny<ArgumentException>(() => _emailCreator.CreateEmails(template));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void CreateEmailsThrowsArgumentExceptionWhenTemplateSubjectIsNullOrEmpty(string subject)
        {
            var template = new EmailTemplate
            {
                Text = "test text",
                Type = "test type",
                Subject = subject
            };

            Assert.ThrowsAny<ArgumentException>(() => _emailCreator.CreateEmails(template));
        }
    }
}
