using Lunitor.Notification.Core.Repository;
using Lunitor.Notification.Web.Endpoints.EmailTypes;
using Lunitor.Notification.Web.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Lunitor.Notification.Web.UnitTests.Endpoints.EmailTypes
{
    public class GetAllTests
    {
        private GetAll _getAll;

        private Mock<IEmailFactoryTypeRepository> _emailFactoryTypeRepositoryMock;

        public GetAllTests()
        {
            _emailFactoryTypeRepositoryMock = new Mock<IEmailFactoryTypeRepository>();

            _getAll = new GetAll(_emailFactoryTypeRepositoryMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullExcpetion_WhenEmailFactoryTypeRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new GetAll(null));
        }


        [Fact]
        public async Task HandleAsync_ReturnsOkWithNotNullEmailTypesResponse()
        {
            var response = await _getAll.HandleAsync();

            Assert.IsType<OkObjectResult>(response.Result);

            var result = response.Result as OkObjectResult;
            Assert.NotNull(result.Value);
            Assert.IsAssignableFrom<IEnumerable<EmailType>>(result.Value);
        }

        [Fact]
        public async Task HandleAsync_ReturnsOkWithCorrectQuantityOfEmailTypes()
        {
            var emailTypes = new EmailType[]
            {
                new EmailType
                {
                    Name = "byuser",
                    Placeholders = new Dictionary<string, string>()
                    {
                        {"UserName","{USERNAME}" },
                    }
                },
                                new EmailType
                {
                    Name = "common",
                    Placeholders = new Dictionary<string, string>()
                }
            };

            _emailFactoryTypeRepositoryMock.Setup(repository => repository.GetAllNames())
                .Returns(emailTypes.Select(t => t.Name));

            _emailFactoryTypeRepositoryMock.Setup(repository => repository.GetPlaceholders(It.IsAny<string>()))
                .Returns((string typeName) =>
                {
                    return emailTypes.First(t => t.Name == typeName).Placeholders;
                });

            var response = await _getAll.HandleAsync();

            var result = response.Result as OkObjectResult;
            var value = result.Value as IEnumerable<EmailType>;
            Assert.Equal(emailTypes.Length, value.Count());
        }
    }
}
