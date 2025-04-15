using RecipesAPI.Model.Ingredients;

namespace RecipesAPI.Model.Recipes
{
    public record GetFullRecipeDTO(Guid Id, string Name,  string Description, GetIngredientDTO[] Ingredients);
}
