using RecipesAPI.Model.Recipes.Get;

namespace RecipesAPI.Services.Interfaces
{
    public interface IRecipeService
    {
        GetRecipeDTO GetRecipeById(Guid recipeId);
        GetRecipeDTO[] GetRecipesByIds(Guid[] recipeIds);

        GetFullRecipeDTO[] GetFullRecipesByIds(Guid[] recipeIds);
        GetFullRecipeDTO GetFullRecipeById(Guid recipeId);

        GetRecipeWithIngredientIdsDTO GetRecipeWithIngredientIds(Guid recipeId);
        GetRecipeWithIngredientIdsDTO[] GetRecipesWithIngredientIdsByIds(Guid[] recipeIds);

        GetRecipeWithIngredientsAndCategoriesDTO GetRecipeWithIngredientsAndCategories(Guid recipeId);

        GetFullRecipeDTO[] GetAllFullRecipes(int count, int page);
        GetFullRecipeDTO[] GetFullRecipesByIngredientIds(int count, int page, Guid[] ingredientIds);
    }
}
