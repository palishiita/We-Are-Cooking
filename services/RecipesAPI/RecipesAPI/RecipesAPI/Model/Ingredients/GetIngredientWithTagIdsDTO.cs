namespace RecipesAPI.Model.Ingredients
{
    public record GetIngredientWithTagIdsDTO(Guid Id, string Name, string Description, Guid[] IngredientTagIds);
}
