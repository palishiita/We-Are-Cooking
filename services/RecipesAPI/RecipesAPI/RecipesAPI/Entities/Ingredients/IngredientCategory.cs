namespace RecipesAPI.Entities.Ingredients
{
    /// <summary>
    /// Tags that define the category of an ingredient.
    /// </summary>
    public class IngredientCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
