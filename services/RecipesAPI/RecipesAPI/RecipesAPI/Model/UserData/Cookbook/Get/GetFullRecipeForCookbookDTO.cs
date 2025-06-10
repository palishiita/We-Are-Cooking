using RecipesAPI.Model.Common;
using RecipesAPI.Model.Ingredients.Get;
using RecipesAPI.Model.Recipes.Get;

namespace RecipesAPI.Model.UserData.Cookbook.Get
{
    public record GetFullRecipeForCookbookDTO(Guid Id, string Name, string Description, IEnumerable<GetRecipeIngredientDTO> Ingredients, bool IsFavorite, CommonUserDataDTO UserData);
}
