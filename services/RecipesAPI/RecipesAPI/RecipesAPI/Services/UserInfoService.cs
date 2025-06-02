using Microsoft.Extensions.Options;
using RecipesAPI.Config.Options;
using RecipesAPI.Exceptions.NotFound;
using RecipesAPI.Model.Common;
using RecipesAPI.Services.Interfaces;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RecipesAPI.Services
{
    public class UserInfoService : IUserInfoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserInfoService> _logger;
        private readonly string _baseRequestUrl;

        public UserInfoService(HttpClient client, ILogger<UserInfoService> logger, IOptions<UserInfoServiceOptions> config)
        {
            _httpClient = client;
            _logger = logger;
            _baseRequestUrl = config.Value.UserInfoServiceUrl;
        }

        public async Task<CommonUserDataDTO> GetUserById(Guid id, Guid requestingUserId)
        {
            var requestUrl = $"{_baseRequestUrl}/profile/{id}";
            _logger.LogInformation("Sending request to: {RequestUrl}", requestUrl);

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("X-Uuid", requestingUserId.ToString());
                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                _logger.LogInformation("Received response: {StatusCode}", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Non-success status code {StatusCode} from UserInfoService", response.StatusCode);
                    return new CommonUserDataDTO(id, "Not Found", string.Empty);
                }

                var responseStream = await response.Content.ReadAsStringAsync();

#if DEBUG
                var userDataTemp = JsonSerializer.Deserialize<UserInfoTempDTO>(responseStream);
                if (userDataTemp == null)
                    throw new UserNotFoundException($"User with id {id} could not be found.");

                return new CommonUserDataDTO(userDataTemp.Id, userDataTemp.Username, userDataTemp.ImageUrl);
#else
                var userData = await JsonSerializer.DeserializeAsync<CommonUserDataDTO>(responseStream);
                if (userData == null)
                    throw new UserNotFoundException($"User with id {id} could not be found.");

                return userData;
#endif
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HttpRequestException while fetching user with id {UserId}", id);
                return new CommonUserDataDTO(id, "Not Found", string.Empty);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error for user ID {UserId}", id);
                return new CommonUserDataDTO(id, "Invalid Data", string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching user with id {UserId}", id);
                throw;
            }
        }

        public record UserInfoTempDTO
        {
            public Guid Id { get; init; }
            public string Username { get; init; }
            public string ImageUrl { get; init; }
            public string ImageSmallUrl { get; init; }
            public bool IsPrivate { get; init; }
            public bool IsBanned { get; init; }
            public string Bio { get; init; }
            public IEnumerable<Guid> Followers { get; init; }
            public IEnumerable<Guid> Recipes { get; init; }
            public IEnumerable<Guid> Reels { get; init; }

            [JsonConstructor]
            public UserInfoTempDTO(
                Guid id,
                string username,
                string imageUrl,
                string imageSmallUrl,
                bool isPrivate,
                bool isBanned,
                string bio,
                IEnumerable<Guid> followers,
                IEnumerable<Guid> recipes,
                IEnumerable<Guid> reels)
            {
                Id = id;
                Username = username;
                ImageUrl = imageUrl;
                ImageSmallUrl = imageSmallUrl;
                IsPrivate = isPrivate;
                IsBanned = isBanned;
                Bio = bio;
                Followers = followers;
                Recipes = recipes;
                Reels = reels;
            }
        }
    }
}
