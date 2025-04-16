namespace RecipesAPI.Model.Ingredients.Get
{
    public record GetIngredientWithCategoryIdsDTO(Guid Id, string Name, string Description, Guid[] IngredientTagIds);
}
