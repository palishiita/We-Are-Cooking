using RecipesAPI.Model.Ingredients.Get;
using RecipesAPI.Model.Recipes.Get;

namespace RecipesAPI.Model.UserData.Fridge.Get
{
    public record GetFullFridgeDTO(Guid Id, Guid UserId, IEnumerable<GetIngredientDTO> Ingredients, IEnumerable<GetRecipeDTO> PossibleRecipes);
}
