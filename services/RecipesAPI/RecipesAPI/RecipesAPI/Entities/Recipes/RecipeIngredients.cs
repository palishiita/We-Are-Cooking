namespace RecipesAPI.Entities.Recipes
{
    /// <summary>
    /// Connection between the recipe and an ingredient.
    /// </summary>
    public class RecipeIngredients
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public Guid IngredientId { get; set; }
    }
}
