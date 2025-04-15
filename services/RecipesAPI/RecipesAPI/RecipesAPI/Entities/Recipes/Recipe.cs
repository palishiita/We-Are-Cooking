using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.Recipes
{
    /// <summary>
    /// A recipe.
    /// </summary>
    [Table("recipes")]
    public class Recipe
    {
        public Recipe()
        {
            Ingredients = new();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual HashSet<RecipeIngredient> Ingredients { get; set; }
    }
}
