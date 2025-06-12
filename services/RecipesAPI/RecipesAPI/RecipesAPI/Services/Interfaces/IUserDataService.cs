using RecipesAPI.Model.Common;
using RecipesAPI.Model.Ingredients.Get;
using RecipesAPI.Model.Recipes.Get;
using RecipesAPI.Model.UserData.Cookbook.Add;
using RecipesAPI.Model.UserData.Cookbook.Get;
using RecipesAPI.Model.UserData.Cookbook.Update;
using RecipesAPI.Model.UserData.Fridge.Add;
using RecipesAPI.Model.UserData.Fridge.Get;

namespace RecipesAPI.Services.Interfaces
{
    public interface IUserDataService
    {
        // cookbook

        Task<PaginatedResult<IEnumerable<GetFullRecipeForCookbookDTO>>> GetFullUserCookbook(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, bool showOnlyFavorites, CancellationToken ct);

        Task AddRecipeToCookbook(Guid userId, AddRecipeToCookbookDTO recipeDTO, CancellationToken ct);
        IEnumerable<Guid> GetIdsOfPresentRecipesInCookbook(Guid userId, IEnumerable<Guid> recipeIds);
        Task ChangeRecipeFavoriteStatus(Guid userId, ChangeRecipeFavoriteStatusDTO statusDTO, CancellationToken ct);
        Task RemoveRecipesFromCookbook(Guid userId, IEnumerable<Guid> recipeIds, CancellationToken ct);

        // fridge

        Task<PaginatedResult<IEnumerable<GetFridgeIngredientDataDTO>>> GetFridgeIngredients(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct);
        Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetRecipesAvailableWithFridge(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct);
        Task SetFridgeIngredients(Guid userId, IEnumerable<SetIngredientQuantityDTO> ingredientsData, CancellationToken ct);

        // this will be done later, when the Units are applied to the recipe ingredients as well
        Task RemoveUsedIngredientsInRecipe(Guid userId, Guid recipeId, CancellationToken ct);

        // restrictions

        Task AddUserRestrictedCategories(Guid userId, IEnumerable<Guid> categoryIds, CancellationToken ct);
        Task RemoveUserRestrictedCategories(Guid userId, IEnumerable<Guid> categoryIds, CancellationToken ct);

        Task<PaginatedResult<IEnumerable<GetIngredientCategoryDTO>>> GetUserRestrictedCategories(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct);
        Task<PaginatedResult<IEnumerable<GetIngredientDTO>>> GetUserRestrictedIngredients(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct);
    }
}
