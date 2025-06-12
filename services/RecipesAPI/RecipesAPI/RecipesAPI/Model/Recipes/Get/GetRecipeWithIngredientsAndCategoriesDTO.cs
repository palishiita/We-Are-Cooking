using RecipesAPI.Model.Common;
using RecipesAPI.Model.Ingredients.Get;

namespace RecipesAPI.Model.Recipes.Get
{
    public record GetRecipeWithIngredientsAndCategoriesDTO(Guid Id, string Name, string Description, IEnumerable<GetFullIngredientDataDTO> IngredientsWithCategories, CommonUserDataDTO UserData);
}
