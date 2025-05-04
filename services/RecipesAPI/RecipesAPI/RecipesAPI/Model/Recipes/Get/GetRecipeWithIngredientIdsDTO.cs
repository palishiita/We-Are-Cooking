using RecipesAPI.Model.Common;

namespace RecipesAPI.Model.Recipes.Get
{
    public record GetRecipeWithIngredientIdsDTO(Guid Id, string Name, string Description, CommonUserDataDTO UserData, IEnumerable<Guid> IngredientIds);
}