using RecipesAPI.Model.Recipes.Add;
using RecipesAPI.Model.Recipes.Get;

namespace RecipesAPI.Services.Interfaces
{
    public interface IRecipeService
    {
        GetRecipeDTO GetRecipeById(Guid recipeId);
        IEnumerable<GetRecipeDTO> GetRecipesByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy);
        IEnumerable<GetRecipeDTO> GetAllRecipes(int count, int page, bool orderByAsc, string sortBy, string query);

        IEnumerable<GetFullRecipeDTO> GetFullRecipesByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy);
        GetFullRecipeDTO GetFullRecipeById(Guid recipeId);

        GetRecipeWithIngredientIdsDTO GetRecipeWithIngredientIds(Guid recipeId);
        IEnumerable<GetRecipeWithIngredientIdsDTO> GetRecipesWithIngredientIdsByIds(IEnumerable<Guid> recipeIds, int count, int page, bool orderByAsc, string sortBy);

        GetRecipeWithIngredientsAndCategoriesDTO GetRecipeWithIngredientsAndCategories(Guid recipeId);

        IEnumerable<GetFullRecipeDTO> GetAllFullRecipes(int count, int page, bool orderByAsc, string sortBy, string query);

        Task<Guid> CreateRecipe(Guid userId, AddRecipeDTO recipeDTO);
        Task<Guid> CreateRecipeWithIngredientsByNames(Guid userId, AddRecipeWithIngredientNamesDTO recipeDTO);
        Task<Guid> CreateRecipeWithIngredientsByIds(Guid userId, AddRecipeWithIngredientIdsDTO recipeDTO);

        Task AddIngredientToRecipeById(Guid recipeId, Guid ingredientId);
        Task AddIngredientsToRecipeById(Guid recipeId, IEnumerable<Guid> ingredientIds);

        Task RemoveIngredientFromRecipe(Guid recipeId, Guid ingredientId);
        Task RemoveIngredientsFromRecipe(Guid recipeId, IEnumerable<Guid> ingredientId);

        Task RemoveRecipeById(Guid recipeId);
    }
}
