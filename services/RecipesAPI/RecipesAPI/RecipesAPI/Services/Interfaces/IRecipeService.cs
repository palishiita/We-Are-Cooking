using RecipesAPI.Model.Common;
using RecipesAPI.Model.Recipes.Add;
using RecipesAPI.Model.Recipes.Get;
using RecipesAPI.Model.Recipes.Update;

namespace RecipesAPI.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<GetRecipeDTO> GetRecipeById(Guid userId, Guid recipeId, CancellationToken ct);
        //Task<PaginatedResult<IEnumerable<GetRecipeDTO>>> GetRecipesByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy, CancellationToken ct);
        Task<PaginatedResult<IEnumerable<GetRecipeDTO>>> GetAllRecipes(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct);

        //Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetFullRecipesByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy, CancellationToken ct);
        Task<GetFullRecipeDTO> GetFullRecipeById(Guid userId, Guid recipeId, CancellationToken ct);

        Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetRecipesWithIngredientIdsByIds(Guid userId, IEnumerable<Guid> recipeIds, int count, int page, CancellationToken ct);

        Task<GetRecipeWithIngredientsAndCategoriesDTO> GetRecipeWithIngredientsAndCategories(Guid userId, Guid recipeId, CancellationToken ct);

        Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetAllFullRecipes(Guid userId, int count, int page, bool orderByAsc, string sortBy, string query, CancellationToken ct);
        Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetUserFullRecipes(Guid userId, Guid selectedUserId, int count, int page, CancellationToken ct);
        // skipped Guid userId
        //Task<PaginatedResult<IEnumerable<GetFullRecipeDTO>>> GetFullRecipesByIngredientIds(IEnumerable<Guid> ingredientIds, int count, int page, bool orderByAsc, string sortBy, CancellationToken ct);

        Task<Guid> CreateRecipeWithIngredientsByIds(Guid userId, AddRecipeWithIngredientsDTO recipeDTO, CancellationToken ct);

        Task UpdateRecipeNameById(Guid userId, Guid recipeId, UpdateRecipeDTO recipeDTO, CancellationToken ct);

        Task AddIngredientToRecipeById(Guid userId, Guid recipeId, AddIngredientToRecipeDTO addIngredientDTO, CancellationToken ct);
        Task<IEnumerable<Guid>> AddIngredientsToRecipeById(Guid userId, Guid recipeId, AddIngredientRangeToRecipeDTO addIngredientsDTO, CancellationToken ct);

        Task RemoveIngredientFromRecipe(Guid userId, Guid recipeId, Guid ingredientId, CancellationToken ct);
        Task RemoveIngredientsFromRecipe(Guid userId, Guid recipeId, IEnumerable<Guid> ingredientId, CancellationToken ct);

        Task RemoveRecipeById(Guid userId, Guid recipeId, CancellationToken ct);

        Task UpdateRecipe(Guid userId, Guid recipeId, AddRecipeWithIngredientsDTO recipeDTO, CancellationToken ct);
    }
}
