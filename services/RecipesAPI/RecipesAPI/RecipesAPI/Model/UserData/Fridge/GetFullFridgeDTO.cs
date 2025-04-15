using RecipesAPI.Model.Ingredients.Get;
using RecipesAPI.Model.Recipes.Get;

namespace RecipesAPI.Model.UserData.Fridge
{
    public record GetFullFridgeDTO(Guid Id, Guid UserId, GetIngredientDTO[] Ingredients, GetRecipeDTO[] PossibleRecipes);
}
