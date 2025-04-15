using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.Ingredients
{
    /// <summary>
    /// Representation of an ingredient.
    /// </summary>
    [Table("ingredients")]
    public class Ingredient
    {
        public Ingredient()
        {
            Connections = new();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual HashSet<IngredientCategoryConnection> Connections { get; set; }
    }
}
