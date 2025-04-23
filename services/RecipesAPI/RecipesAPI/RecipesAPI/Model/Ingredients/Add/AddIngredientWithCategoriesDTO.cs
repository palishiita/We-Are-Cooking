namespace RecipesAPI.Model.Ingredients.Add
{
    public record AddIngredientWithCategoriesDTO(string Name, string Description, IEnumerable<string> IngredientCategories);
}
