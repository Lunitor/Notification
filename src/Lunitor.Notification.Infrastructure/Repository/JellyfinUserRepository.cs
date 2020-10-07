using Ardalis.GuardClauses;
using Lunitor.Notification.Core.Model;
using Lunitor.Notification.Core.Repository;
using Lunitor.Notification.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lunitor.Notification.Infrastructure.Repository
{
    internal class JellyfinUserRepository : IUserRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JellyfinUserRepositoryConfiguration _configuration;
        private readonly ILogger<JellyfinUserRepository> _logger;

        public JellyfinUserRepository(HttpClient httpClient, IOptions<JellyfinUserRepositoryConfiguration> configuration, ILogger<JellyfinUserRepository> logger)
        {
            Guard.Against.Null(httpClient, nameof(httpClient));
            Guard.Against.Null(logger, nameof(logger));
            Guard.Against.Null(configuration, nameof(configuration));

            _httpClient = httpClient;
            _configuration = configuration.Value;
            _logger = logger;

            _httpClient.DefaultRequestHeaders.Add("X-Emby-Token", _configuration.JellyfinAccessToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(_configuration.JellyfinUserEmailEnpoint);

                var jellyfinUsers = JsonSerializer.Deserialize<IEnumerable<JellyfinUserDto>>(response);

                return jellyfinUsers.Select(ju => ju.Map());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Jellyfin users from {endpoint}!", _configuration.JellyfinUserEmailEnpoint);

                return new List<User>();
            }
        }
    }
}
