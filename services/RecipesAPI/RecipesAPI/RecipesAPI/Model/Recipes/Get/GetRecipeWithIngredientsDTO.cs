namespace RecipesAPI.Model.Recipes.Get
{
    public record GetRecipeWithIngredientsDTO(Guid Id, string Name, string Description, string[] Ingredients);
}
