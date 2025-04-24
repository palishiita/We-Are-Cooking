using RecipesAPI.Model.Recipes.Add;
using RecipesAPI.Model.Recipes.Get;

namespace RecipesAPI.Services.Interfaces
{
    public interface IRecipeService
    {
        GetRecipeDTO GetRecipeById(Guid recipeId);
        IEnumerable<GetRecipeDTO> GetRecipesByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy, string query);
        IEnumerable<GetRecipeDTO> GetAllRecipes(int count, int page, bool orderByAsc, string sortBy, string query);

        IEnumerable<GetFullRecipeDTO> GetFullRecipesByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy, string query);
        GetFullRecipeDTO GetFullRecipeById(Guid recipeId);

        GetRecipeWithIngredientIdsDTO GetRecipeWithIngredientIds(Guid recipeId);
        IEnumerable<GetRecipeWithIngredientIdsDTO> GetRecipesWithIngredientIdsByIds(IEnumerable<Guid> recipeIds, bool orderByAsc, string sortBy, string query);

        GetRecipeWithIngredientsAndCategoriesDTO GetRecipeWithIngredientsAndCategories(Guid recipeId);

        IEnumerable<GetFullRecipeDTO> GetAllFullRecipes(int count, int page, bool orderByAsc, string sortBy, string query);
        IEnumerable<GetFullRecipeDTO> GetFullRecipesByIngredientIds(IEnumerable<Guid> ingredientIds, int count, int page, bool orderByAsc, string sortBy, string query);

        Task<Guid> CreateRecipe(AddRecipeDTO recipeDTO);
        Task<Guid> CreateRecipeWithIngredientsByNames(AddRecipeWithIngredientNamesDTO recipeDTO);
        Task<Guid> CreateRecipeWithIngredientsByIds(AddRecipeWithIngredientIdsDTO recipeDTO);
    }
}
