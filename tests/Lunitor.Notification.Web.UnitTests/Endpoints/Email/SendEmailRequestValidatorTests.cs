using FluentValidation.TestHelper;
using Lunitor.Notification.Core.Repository;
using Lunitor.Notification.Web.Endpoints.Email;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

namespace Lunitor.Notification.Web.UnitTests.Endpoints.Email
{
    public class SendEmailRequestValidatorTests
    {
        private SendEmailRequestValidator _validator;

        private Mock<IEmailFactoryTypeRepository> _emailFactoryTypeRepositoryMock;

        public SendEmailRequestValidatorTests()
        {
            _emailFactoryTypeRepositoryMock = new Mock<IEmailFactoryTypeRepository>();
            _emailFactoryTypeRepositoryMock.Setup(r => r.GetAllNames())
                .Returns(Enumerable.Empty<string>());

            _validator = new SendEmailRequestValidator(_emailFactoryTypeRepositoryMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenEmailFactoryTypeRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SendEmailRequestValidator(null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("       ")]
        public void HasValidationErrorForType_WhenTypeIsNullEmptyOnlyWhiteSpace(string type)
        {
            var request = new SendEmailRequest
            {
                Type = type,
                Subject = "test subject",
                Body = "test"
            };

            var validationResult = _validator.TestValidate(request);

            validationResult.ShouldHaveValidationErrorFor(r => r.Type);
        }

        [Fact]
        public void HasValidationErrorForType_WhenTypeHaseNoEmailFactoryTypeEquivalent()
        {
            _emailFactoryTypeRepositoryMock.Setup(r => r.GetAllNames())
                .Returns(new List<string>
                {
                    "EmailFactoryOne",
                    "EmailFactoryTwo"
                });

            var request = new SendEmailRequest
            {
                Type = "type",
                Subject = "test subject",
                Body = "test"
            };

            var validationResult = _validator.TestValidate(request);

            validationResult.ShouldHaveValidationErrorFor(r => r.Type);
        }

        [Fact]
        public void HasNoValidationErrorForType_WhenTypeHasEmailFactoryTypeEquivalent()
        {
            _emailFactoryTypeRepositoryMock.Setup(r => r.GetAllNames())
                .Returns(new List<string>
                {
                    "EmailFactoryOne",
                    "EmailFactoryTwo"
                });

            var request = new SendEmailRequest
            {
                Type = "EmailFactoryOne",
                Subject = "test subject",
                Body = "test"
            };

            var validationResult = _validator.TestValidate(request);

            validationResult.ShouldNotHaveValidationErrorFor(r => r.Type);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("       ")]
        public void HasValidationErrorForSubject_WhenSubjectIsNullEmptyOnlyWhiteSpace(string subject)
        {
            var request = new SendEmailRequest
            {
                Type = "type",
                Subject = subject,
                Body = "test"
            };

            var validationResult = _validator.TestValidate(request);

            validationResult.ShouldHaveValidationErrorFor(r => r.Subject);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("       ")]
        public void HasValidationErrorForBody_WhenBodyIsNullEmptyOnlyWhiteSpace(string body)
        {
            var request = new SendEmailRequest
            {
                Type = "type",
                Subject = "subject",
                Body = body
            };

            var validationResult = _validator.TestValidate(request);

            validationResult.ShouldHaveValidationErrorFor(r => r.Body);
        }
    }
}
