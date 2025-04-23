namespace RecipesAPI.Model.Recipes.Add
{
    public record AddRecipeWithIngredientNamesDTO(string Name, string Description, IEnumerable<string> Ingredients);
}
