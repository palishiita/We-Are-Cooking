namespace RecipesAPI.Model.Recipes.Add
{
    public record AddIngredientRangeToRecipeDTO(IEnumerable<Guid> IngredientIds);
}
