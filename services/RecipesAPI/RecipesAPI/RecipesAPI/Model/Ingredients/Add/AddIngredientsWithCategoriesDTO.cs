namespace RecipesAPI.Model.Ingredients.Add
{
    public record AddIngredientsWithCategoriesDTO(string Name, string Description, IEnumerable<string> IngredientCategories);
}
