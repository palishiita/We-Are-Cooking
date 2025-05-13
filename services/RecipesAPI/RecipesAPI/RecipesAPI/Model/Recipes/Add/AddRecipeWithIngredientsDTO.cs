namespace RecipesAPI.Model.Recipes.Add
{
    public record AddRecipeWithIngredientsDTO(string Name, string Description, IEnumerable<AddIngredientToRecipeDTO> Ingredients);
}
