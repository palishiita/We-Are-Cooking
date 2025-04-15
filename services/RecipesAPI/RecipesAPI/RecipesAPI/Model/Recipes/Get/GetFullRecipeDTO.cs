using RecipesAPI.Model.Ingredients.Get;

namespace RecipesAPI.Model.Recipes.Get
{
    public record GetFullRecipeDTO(Guid Id, string Name, string Description, GetIngredientDTO[] Ingredients);
}
