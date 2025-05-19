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
        private readonly string _requestUrl;

        public UserInfoService(HttpClient client, ILogger<UserInfoService> logger)
        {
            _httpClient = client;
            _logger = logger;
            _requestUrl = "http://userservice..."; // to be checked
        }

        public async Task<CommonUserDataDTO> GetUserById(Guid id)
        {
            try
            {
                var userURL = string.Format("{0}/{1}", _requestUrl, id);
                var response = await _httpClient.GetAsync(_requestUrl);

#if DEBUG
                var responseValue = await response.Content.ReadAsStringAsync();
                var userData = JsonSerializer.Deserialize<CommonUserDataDTO>(responseValue)
                    ?? throw new UserNotFoundException($"User with id {id} could not be found.");
#else
                var userData = JsonSerializer.Deserialize<CommonUserDataDTO>(await response.Content.ReadAsStringAsync()) 
                    ?? throw new UserNotFoundException($"User with id {id} could not be found.");
#endif
                return userData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception: {ex.Message}");
                throw;
            }
        }
    }
}
