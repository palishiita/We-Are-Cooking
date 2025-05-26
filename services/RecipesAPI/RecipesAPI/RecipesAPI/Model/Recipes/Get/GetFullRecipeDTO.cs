using RecipesAPI.Model.Common;

namespace RecipesAPI.Model.Recipes.Get
{
    public record GetFullRecipeDTO(Guid Id, string Name, string Description, IEnumerable<GetRecipeIngredientDTO> Ingredients, CommonUserDataDTO UserData);
}
