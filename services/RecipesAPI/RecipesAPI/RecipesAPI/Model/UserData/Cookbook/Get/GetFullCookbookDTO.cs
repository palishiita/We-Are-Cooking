using RecipesAPI.Model.Recipes.Get;

namespace RecipesAPI.Model.UserData.Cookbook.Get
{
    public record GetFullCookbookDTO(Guid Id, Guid UserId, IEnumerable<GetRecipeDTO> CookbookRecipes);
}
