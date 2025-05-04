using RecipesAPI.Model.Common;
using RecipesAPI.Model.Ingredients.Get;

namespace RecipesAPI.Model.UserData.Cookbook.Get
{
    public record GetFullRecipeForCookbookDTO(string Name, string Description, IEnumerable<GetIngredientDTO> Ingredients, bool IsFavorite, CommonUserDataDTO UserData);
}
