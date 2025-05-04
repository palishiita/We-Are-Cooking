namespace RecipesAPI.Model.Ingredients.Get
{
    public record GetIngredientWithCategoryIdsDTO(Guid Id, string Name, string Description, IEnumerable<Guid> IngredientTagIds);
}
