namespace RecipesAPI.Model.Recipes.Get
{
    public record GetRecipeWithIngredientIdsDTO(Guid Id, string Name, string Description, IEnumerable<Guid> IngredientIds);
}