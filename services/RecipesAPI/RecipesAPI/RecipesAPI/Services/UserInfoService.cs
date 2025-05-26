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

            // NOT WORKING USER_INFO_SERVICE YET
            return new CommonUserDataDTO(id, "Temporary", "Disabled");

            try
            {
                var requestURL = string.Format("{0}/{1}", _baseRequestUrl, id);
                var response = await _httpClient.GetAsync(requestURL);

#if DEBUG
                var responseValue = await response.Content.ReadAsStringAsync();
                var userDataMapped = JsonSerializer.Deserialize<UserInfoTempDTO>(responseValue)
                    ?? throw new UserNotFoundException($"User with id {id} could not be found.");
#else
                var userData = JsonSerializer.Deserialize<UserInfoTempDTO>(await response.Content.ReadAsStringAsync()) 
                    ?? throw new UserNotFoundException($"User with id {id} could not be found.");
#endif
                var userData = new CommonUserDataDTO(userDataMapped.Id, userDataMapped.Username, userDataMapped.PhotoUrl);

                return userData;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}");
                return new CommonUserDataDTO(id, "Not Found", string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}");
                throw;
            }
        }

        private record UserInfoTempDTO(Guid Id, string Username, string PhotoUrl, string Bio);
    }
}
