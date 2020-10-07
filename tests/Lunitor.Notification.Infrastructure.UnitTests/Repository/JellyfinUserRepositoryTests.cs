using Lunitor.Notification.Infrastructure.Configuration;
using Lunitor.Notification.Infrastructure.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Lunitor.Notification.Infrastructure.UnitTests.Repository
{
    public class JellyfinUserRepositoryTests
    {
        private JellyfinUserRepository _jellyfinUserRepository;

        private MockHttpMessageHandler _messageHandlerMock;
        private Mock<IOptions<JellyfinUserRepositoryConfiguration>> _configurationMock;
        private Mock<ILogger<JellyfinUserRepository>> _loggerMock;

        private readonly JellyfinUserRepositoryConfiguration TestConfiguration = new JellyfinUserRepositoryConfiguration
        {
            JellyfinUserEmailEnpoint = "http://jellyfin.net/users/endpoint",
            JellyfinAccessToken = "accesstoken"
        };

        private const string JsonMediaType = "application/json";
        private const string TestJellyfinUserResponse =
        @"[
            {
                ""UserId"": ""0ib6d7a0823d4cb49cd47f4b7aef004f"",
                ""Username"": ""TestUser"",
                ""EmailAddress"": ""test.user@test.net""
            },
            {
                ""UserId"": ""29v7081ba00d4192a899ceadfc538565"",
                ""Username"": ""TestUser2"",
                ""EmailAddress"": ""test.user2@test.net""
            }
        ]";

        public JellyfinUserRepositoryTests()
        {
            _messageHandlerMock = new MockHttpMessageHandler();

            _configurationMock = new Mock<IOptions<JellyfinUserRepositoryConfiguration>>();
            _configurationMock.SetupGet(c => c.Value)
                .Returns(TestConfiguration);

            _loggerMock = new Mock<ILogger<JellyfinUserRepository>>();

            var httpClient = new HttpClient(_messageHandlerMock);
            _jellyfinUserRepository = new JellyfinUserRepository(httpClient, _configurationMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenHttpClientIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new JellyfinUserRepository(null, _configurationMock.Object, _loggerMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenJellfinUserRepositoryConfigurationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new JellyfinUserRepository(new HttpClient(_messageHandlerMock), null,_loggerMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new JellyfinUserRepository(new HttpClient(_messageHandlerMock), _configurationMock.Object, null));
        }

        [Fact]
        public async Task GetAllAsync_RequestUsesJellyfinAccessTokenHeader()
        {
            var request = _messageHandlerMock.When("*")
                .WithHeaders("X-Emby-Token", TestConfiguration.JellyfinAccessToken)
                .Respond(JsonMediaType, "[]");

            await _jellyfinUserRepository.GetAllAsync();

            Assert.Equal(1, _messageHandlerMock.GetMatchCount(request));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyIEnumerable_IfHttpClientThrowsErrorDuringRequest()
        {
            _messageHandlerMock.When("*")
                .Throw(new Exception());

            var users = await _jellyfinUserRepository.GetAllAsync();

            Assert.Empty(users);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyIEnumerable_IfHttpClientReturnsNoUsers()
        {
            _messageHandlerMock.When("*")
                .Respond(JsonMediaType, "[]");

            var users = await _jellyfinUserRepository.GetAllAsync();

            Assert.Empty(users);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyIEnumerable_IfHttpClientReturnsEmptyResponse()
        {
            _messageHandlerMock.When("*")
                .Respond(JsonMediaType, "");

            var users = await _jellyfinUserRepository.GetAllAsync();

            Assert.Empty(users);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsNotEmptyIEnumerable_IfHttpClientReturnsUsersFromEndpoint()
        {
            _messageHandlerMock.When("*")
                .Respond(JsonMediaType, TestJellyfinUserResponse);

            var users = await _jellyfinUserRepository.GetAllAsync();

            Assert.NotEmpty(users);
        }
    }
}
