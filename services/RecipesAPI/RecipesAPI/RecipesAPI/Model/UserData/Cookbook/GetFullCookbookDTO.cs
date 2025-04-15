using RecipesAPI.Model.Recipes;

namespace RecipesAPI.Model.UserData.Cookbook
{
    public record GetFullCookbookDTO(Guid Id, Guid UserId, GetRecipeDTO[] CookbookRecipes);
}
