namespace RecipesAPI.Model.Recipes
{
    public record GetRecipeWithIngredientIdsDTO(Guid Id, string Name, string Description, Guid[] IngredientIds);
}