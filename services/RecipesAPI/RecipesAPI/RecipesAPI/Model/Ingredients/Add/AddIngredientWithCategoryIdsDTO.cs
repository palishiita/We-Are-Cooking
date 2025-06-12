namespace RecipesAPI.Model.Ingredients.Add
{
    public record AddIngredientWithCategoryIdsDTO(string Name, string Description, IEnumerable<Guid> CategoryIds);
}
