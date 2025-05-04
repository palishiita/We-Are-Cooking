using RecipesAPI.Model.UserData.Cookbook.Add;
using RecipesAPI.Model.UserData.Cookbook.Get;
using RecipesAPI.Model.UserData.Cookbook.Update;

namespace RecipesAPI.Services.Interfaces
{
    public interface IUserDataService
    {
        IEnumerable<GetFullRecipeForCookbookDTO> GetFullUserCookbook(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, bool showOnlyFavorites);

        Task AddRecipeToCookbook(Guid userId, AddRecipeToCookbookDTO recipeDTO);
        Task ChangeRecipeFavoriteStatus(Guid userId, ChangeRecipeFavoriteStatusDTO statusDTO);
        Task RemoveRecipesFromCookbook(Guid userId, IEnumerable<Guid> recipeIds);
    }
}
