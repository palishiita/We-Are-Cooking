namespace RecipesAPI.Entities.Ingredients
{
    /// <summary>
    /// A connection between ingredient and its tag.
    /// </summary>
    public class IngredientCategoryConnection
    {
        public Guid Id { get; set; }
        public Guid IngredientTagId { get; set; }
        public Guid IngredientId { get; set; }
    }
}
