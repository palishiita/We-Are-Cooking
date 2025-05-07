using RecipesAPI.Model.Common;
using RecipesAPI.Model.Ingredients.Get;
using RecipesAPI.Model.Recipes.Get;
using RecipesAPI.Model.UserData.Cookbook.Add;
using RecipesAPI.Model.UserData.Cookbook.Get;
using RecipesAPI.Model.UserData.Cookbook.Update;
using RecipesAPI.Model.UserData.Fridge.Add;
using RecipesAPI.Model.UserData.Fridge.Delete;
using RecipesAPI.Model.UserData.Fridge.Get;

namespace RecipesAPI.Services.Interfaces
{
    public interface IUserDataService
    {
        // cookbook

        Task<PaginatedResult<IEnumerable<GetFullRecipeForCookbookDTO>>> GetFullUserCookbook(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, bool showOnlyFavorites);

        Task AddRecipeToCookbook(Guid userId, AddRecipeToCookbookDTO recipeDTO);
        Task ChangeRecipeFavoriteStatus(Guid userId, ChangeRecipeFavoriteStatusDTO statusDTO);
        Task RemoveRecipesFromCookbook(Guid userId, IEnumerable<Guid> recipeIds);

        // fridge

        Task<PaginatedResult<IEnumerable<GetFridgeIngredientDataDTO>>> GetFridgeIngredients(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query);
        Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetRecipesAvailableWithFridge(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query);
        Task UpdateFridgeIngredients(Guid userId, IEnumerable<SetIngredientQuantityDTO> ingredientsData);
        Task RemoveIngredientsFromFridge(Guid userId, IEnumerable<RemoveIngredientQuantityDTO> ingredientsData);

        // this will be done later, when the Units are applied to the recipe ingredients as well
        Task RemoveUsedIngredientsInRecipe(Guid userId, Guid recipeId);

        // restrictions
    }
}
