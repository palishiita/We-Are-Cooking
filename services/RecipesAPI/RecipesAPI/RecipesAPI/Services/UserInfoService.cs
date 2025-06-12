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
            if(id == Guid.Empty)
            {
                return new CommonUserDataDTO(id, "Not Found", string.Empty);
            }

            var requestUrl = $"{_baseRequestUrl}/{id}";
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

                await using var responseStream = await response.Content.ReadAsStreamAsync();
                var userDataTemp = await JsonSerializer.DeserializeAsync<UserInfoTempDTO>(responseStream);
                if (userDataTemp == null)
                    throw new UserNotFoundException($"User with id {id} could not be found.");

                _logger.LogError($"TEMP: {userDataTemp.ToString()}");

                return new CommonUserDataDTO(userDataTemp.Id, userDataTemp.Username, userDataTemp.ImageUrl);
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
            [JsonPropertyName("userUuid")]
            public Guid Id { get; init; }
            [JsonPropertyName("userName")]
            public string Username { get; init; }
            [JsonPropertyName("imageUrl")]
            public string ImageUrl { get; init; }
            [JsonPropertyName("imageSmallUrl")]
            public string ImageSmallUrl { get; init; }
            [JsonPropertyName("isPrivate")]
            public bool IsPrivate { get; init; }
            [JsonPropertyName("isBanned")]
            public bool IsBanned { get; init; }
            [JsonPropertyName("bio")]
            public string Bio { get; init; }
            [JsonPropertyName("followers")]
            public IEnumerable<Guid> Followers { get; init; }
            [JsonPropertyName("recipes")]
            public IEnumerable<Guid> Recipes { get; init; }
            [JsonPropertyName("reels")]
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
