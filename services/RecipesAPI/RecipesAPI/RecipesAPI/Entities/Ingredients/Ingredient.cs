namespace RecipesAPI.Entities.Ingredients
{
    /// <summary>
    /// Representation of an ingredient.
    /// </summary>
    public class Ingredient
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
