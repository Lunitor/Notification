using Lunitor.Notification.Core.Repository;
using Lunitor.Notification.Web.Endpoints.ArchiveEmailTemplate;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Lunitor.Notification.Web.UnitTests.Endpoints.ArchiveEmailTemplate
{
    public class GetAllTests
    {
        private GetAll _getAll;

        private Mock<IArchiveEmailTemplateRepository> _archiveEmailTemplateRepositoryMock;

        public GetAllTests()
        {
            _archiveEmailTemplateRepositoryMock = new Mock<IArchiveEmailTemplateRepository>();

            _getAll = new GetAll(_archiveEmailTemplateRepositoryMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfArchiveEmailTemplateRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new GetAll(null));
        }

        [Fact]
        public async Task HandleAsync_ReturnsOkWithNotNullArchiveEmailTemplatesResponse()
        {
            var response = await _getAll.HandleAsync();

            Assert.IsType<OkObjectResult>(response.Result);

            var result = response.Result as OkObjectResult;
            Assert.NotNull(result.Value);
            Assert.IsAssignableFrom<IEnumerable<Core.Model.ArchiveEmailTemplate>>(result.Value);
        }

        [Fact]
        public async Task HandleAsync_ReturnsOkWithCorrectQuantityOfArchiveEmailTemplates()
        {
            var archiveEmailTemplates = new Core.Model.ArchiveEmailTemplate[]
            {
                new Core.Model.ArchiveEmailTemplate(),
                new Core.Model.ArchiveEmailTemplate()
            };

            _archiveEmailTemplateRepositoryMock.Setup(repository => repository.GetAll())
                .Returns(archiveEmailTemplates);

            var response = await _getAll.HandleAsync();

            var result = response.Result as OkObjectResult;
            var value = result.Value as IEnumerable<Core.Model.ArchiveEmailTemplate>;
            Assert.Equal(archiveEmailTemplates.Length, value.Count());
        }
    }
}
