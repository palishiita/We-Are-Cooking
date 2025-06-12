using RecipesAPI.Model.Common;

namespace RecipesAPI.Model.Recipes.Get
{
    public record GetRecipeDTO(Guid Id, string Name, string Description, CommonUserDataDTO UserData);
}
