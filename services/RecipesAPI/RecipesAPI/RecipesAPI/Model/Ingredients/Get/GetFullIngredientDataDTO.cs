namespace RecipesAPI.Model.Ingredients.Get
{
    public record GetFullIngredientDataDTO(Guid Id, string Name, string Description, GetIngredientCategoryDTO[] IngredientCategories);
}
