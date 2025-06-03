using RecipesAPI.Model.Common;

namespace RecipesAPI.Services.Interfaces
{
    public interface IUserInfoService
    {
        Task<CommonUserDataDTO> GetUserById(Guid id, Guid requestingUserId);
    }
}
