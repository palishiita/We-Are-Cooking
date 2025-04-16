namespace RecipesAPI.Model.Ingredients.Get
{
    public record GetIngredientWithCategories(Guid Id, string Name, string Description, string[] Categories);
}
