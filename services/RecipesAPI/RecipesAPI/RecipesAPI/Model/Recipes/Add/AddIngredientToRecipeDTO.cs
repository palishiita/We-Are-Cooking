namespace RecipesAPI.Model.Recipes.Add
{
    public record AddIngredientToRecipeDTO(Guid IngredientId, double Quantity, Guid UnitId);
}
