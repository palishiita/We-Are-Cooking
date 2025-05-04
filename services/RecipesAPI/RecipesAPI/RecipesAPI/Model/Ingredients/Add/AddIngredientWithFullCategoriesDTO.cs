namespace RecipesAPI.Model.Ingredients.Add
{
    public record AddIngredientWithFullCategoriesDTO(string Name, string Description, IEnumerable<AddIngredientCategoryDTO> IngredientCategories);
}
