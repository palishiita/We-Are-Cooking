using RecipesAPI.Model.Common;
using RecipesAPI.Model.Recipes.Add;
using RecipesAPI.Model.Recipes.Get;
using RecipesAPI.Model.Recipes.Update;

namespace RecipesAPI.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<GetRecipeDTO> GetRecipeById(Guid recipeId, CancellationToken ct);
        Task<PaginatedResult<IEnumerable<GetRecipeDTO>>> GetRecipesByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy, CancellationToken ct);
        Task<PaginatedResult<IEnumerable<GetRecipeDTO>>> GetAllRecipes(int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct);

        Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetFullRecipesByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy, CancellationToken ct);
        Task<GetFullRecipeDTO> GetFullRecipeById(Guid recipeId, CancellationToken ct);

        Task<PaginatedResult<IEnumerable<GetRecipeWithIngredientIdsDTO>>> GetRecipesWithIngredientIdsByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy, CancellationToken ct);

        Task<GetRecipeWithIngredientsAndCategoriesDTO> GetRecipeWithIngredientsAndCategories(Guid recipeId, CancellationToken ct);

        Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetAllFullRecipes(int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct);
        Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetFullRecipesByIngredientIds(IEnumerable<Guid> ingredientIds, int count, int page, bool orderByAsc, string sortBy, CancellationToken ct);

        Task<Guid> CreateRecipeWithIngredientsByIds(Guid userId, AddRecipeWithIngredientsDTO recipeDTO, CancellationToken ct);

        Task UpdateRecipeNameById(Guid recipeId, UpdateRecipeDTO recipeDTO, CancellationToken ct);

        Task AddIngredientToRecipeById(Guid recipeId, AddIngredientToRecipeDTO addIngredientDTO, CancellationToken ct);
        Task<IEnumerable<Guid>> AddIngredientsToRecipeById(Guid recipeId, AddIngredientRangeToRecipeDTO addIngredientsDTO, CancellationToken ct);

        Task RemoveIngredientFromRecipe(Guid recipeId, Guid ingredientId, CancellationToken ct);
        Task RemoveIngredientsFromRecipe(Guid recipeId, IEnumerable<Guid> ingredientId, CancellationToken ct);

        Task RemoveRecipeById(Guid recipeId, CancellationToken ct);
    }
}
