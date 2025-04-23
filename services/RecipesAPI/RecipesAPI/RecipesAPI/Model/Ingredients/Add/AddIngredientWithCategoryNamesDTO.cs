namespace RecipesAPI.Model.Ingredients.Add
{
    public record AddIngredientWithCategoryNamesDTO(string Name, string Description, IEnumerable<string> IngredientCategories);
}
