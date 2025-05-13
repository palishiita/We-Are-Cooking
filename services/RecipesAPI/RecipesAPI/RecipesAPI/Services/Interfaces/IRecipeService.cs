using RecipesAPI.Model.Common;
using RecipesAPI.Model.Recipes.Add;
using RecipesAPI.Model.Recipes.Get;
using RecipesAPI.Model.Recipes.Update;

namespace RecipesAPI.Services.Interfaces
{
    public interface IRecipeService
    {
        GetRecipeDTO GetRecipeById(Guid recipeId);
        Task<PaginatedResult<IEnumerable<GetRecipeDTO>>> GetRecipesByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy);
        Task<PaginatedResult<IEnumerable<GetRecipeDTO>>> GetAllRecipes(int count, int page, bool orderByAsc, string sortBy, string query);

        Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetFullRecipesByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy);
        GetFullRecipeDTO GetFullRecipeById(Guid recipeId);

        GetRecipeWithIngredientIdsDTO GetRecipeWithIngredientIds(Guid recipeId);
        Task<PaginatedResult<IEnumerable<GetRecipeWithIngredientIdsDTO>>> GetRecipesWithIngredientIdsByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy);

        GetRecipeWithIngredientsAndCategoriesDTO GetRecipeWithIngredientsAndCategories(Guid recipeId);

        Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetAllFullRecipes(int count, int page, bool orderByAsc, string sortBy, string query);
        Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetFullRecipesByIngredientIds(IEnumerable<Guid> ingredientIds, int count, int page, bool orderByAsc, string sortBy);

        Task<Guid> CreateRecipeWithIngredientsByIds(Guid userId, AddRecipeWithIngredientsDTO recipeDTO);

        Task UpdateRecipeNameById(Guid recipeId, UpdateRecipeDTO recipeDTO);

        Task AddIngredientToRecipeById(Guid recipeId, AddIngredientToRecipeDTO addIngredientDTO);
        Task<IEnumerable<Guid>> AddIngredientsToRecipeById(Guid recipeId, AddIngredientRangeToRecipeDTO addIngredientsDTO);

        Task RemoveIngredientFromRecipe(Guid recipeId, Guid ingredientId);
        Task RemoveIngredientsFromRecipe(Guid recipeId, IEnumerable<Guid> ingredientId);

        Task RemoveRecipeById(Guid recipeId);
    }
}
