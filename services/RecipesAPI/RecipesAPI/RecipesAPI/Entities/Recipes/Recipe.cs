namespace RecipesAPI.Entities.Recipes
{
    /// <summary>
    /// A recipe.
    /// </summary>
    public class Recipe
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
