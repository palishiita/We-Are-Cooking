namespace RecipesAPI.Model.Ingredients.Get
{
    public record GetIngredientWithTagIdsDTO(Guid Id, string Name, string Description, Guid[] IngredientTagIds);
}
