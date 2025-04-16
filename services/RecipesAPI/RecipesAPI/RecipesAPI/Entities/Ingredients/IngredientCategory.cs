using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.Ingredients
{
    /// <summary>
    /// Tags that define the category of an ingredient.
    /// </summary>
    [Table("ingredient_categories")]
    public class IngredientCategory : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        public virtual HashSet<IngredientCategoryConnection> Connections { get; set; }
    }
}
