using RecipesAPI.Model.Recipes.Get;

namespace RecipesAPI.Services.Interfaces
{
    public interface IRecipeService
    {
        GetRecipeDTO GetRecipeById(Guid recipeId);
        GetRecipeDTO[] GetRecipesByIds(Guid[] recipeIds, int count, int page, bool orderByAsc, string sortBy);

        GetFullRecipeDTO[] GetFullRecipesByIds(Guid[] recipeIds, int count, int page, bool orderByAsc, string sortBy);
        GetFullRecipeDTO GetFullRecipeById(Guid recipeId);

        GetRecipeWithIngredientIdsDTO GetRecipeWithIngredientIds(Guid recipeId);
        GetRecipeWithIngredientIdsDTO[] GetRecipesWithIngredientIdsByIds(Guid[] recipeIds, bool orderByAsc, string sortBy);

        GetRecipeWithIngredientsAndCategoriesDTO GetRecipeWithIngredientsAndCategories(Guid recipeId);

        GetFullRecipeDTO[] GetAllFullRecipes(int count, int page, bool orderByAsc, string sortBy);
        GetFullRecipeDTO[] GetFullRecipesByIngredientIds(Guid[] ingredientIds, int count, int page, bool orderByAsc, string sortBy);
    }
}
