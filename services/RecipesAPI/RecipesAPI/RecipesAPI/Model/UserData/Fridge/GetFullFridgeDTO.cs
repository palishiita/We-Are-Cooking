using RecipesAPI.Model.Ingredients;
using RecipesAPI.Model.Recipes;

namespace RecipesAPI.Model.UserData.Fridge
{
    public record GetFullFridgeDTO(Guid Id, Guid UserId, GetIngredientDTO[] Ingredients, GetRecipeDTO[] PossibleRecipes);
}
