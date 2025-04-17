using RecipesAPI.Model.Ingredients.Get;

namespace RecipesAPI.Model.Recipes.Get
{
    public record GetRecipeWithIngredientsAndCategoriesDTO(Guid Id, string Name, string Description, GetFullIngredientDataDTO[] IngredientsWithCategories);
}
