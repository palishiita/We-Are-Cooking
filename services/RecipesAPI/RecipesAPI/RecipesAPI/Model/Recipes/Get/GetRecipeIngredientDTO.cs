namespace RecipesAPI.Model.Recipes.Get
{
    public record GetRecipeIngredientDTO(Guid IngredientId, string Name, string Description, double Quantity, Guid UnitId, string Unit);
}
