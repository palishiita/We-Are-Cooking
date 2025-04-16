using RecipesAPI.Model.Ingredients.Get;

namespace RecipesAPI.Model.Recipes.Get
{
    public record GetRecipeWithIngredientsAndCategoriesDTO(Guid Id, GetFullIngredientDataDTO[] IngredientsWithCategories);
}
