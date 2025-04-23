namespace RecipesAPI.Model.Recipes.Add
{
    public record AddRecipeWithIngredientIdsDTO(string Name, string Description, IEnumerable<Guid> IngredientIds);
}
