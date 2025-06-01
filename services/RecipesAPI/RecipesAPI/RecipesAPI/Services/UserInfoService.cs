using Microsoft.Extensions.Options;
using RecipesAPI.Config.Options;
using RecipesAPI.Exceptions.NotFound;
using RecipesAPI.Model.Common;
using RecipesAPI.Services.Interfaces;
using System.Text.Json;

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

        public async Task<CommonUserDataDTO> GetUserById(Guid id)
        {
            var requestUrl = $"{_baseRequestUrl}/profile/{id}";
            _logger.LogInformation("Sending request to: {RequestUrl}", requestUrl);

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                _logger.LogInformation("Received response: {StatusCode}", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Non-success status code {StatusCode} from UserInfoService", response.StatusCode);
                    return new CommonUserDataDTO(id, "Not Found", string.Empty);
                }

                await using var responseStream = await response.Content.ReadAsStreamAsync();

#if DEBUG
                var userDataTemp = await JsonSerializer.DeserializeAsync<UserInfoTempDTO>(responseStream);
                if (userDataTemp == null)
                    throw new UserNotFoundException($"User with id {id} could not be found.");

                return new CommonUserDataDTO(userDataTemp.Id, userDataTemp.Username, userDataTemp.PhotoUrl);
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

        private record UserInfoTempDTO(Guid Id, string Username, string PhotoUrl, string Bio);
    }
}
