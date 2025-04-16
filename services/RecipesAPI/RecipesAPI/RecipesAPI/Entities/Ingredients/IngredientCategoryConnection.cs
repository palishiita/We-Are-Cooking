using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.Ingredients
{
    /// <summary>
    /// A connection between ingredient and its tag.
    /// </summary>
    [Table("ingredient_categories_connections")]
    public class IngredientCategoryConnection : IEntity
    {
        public Guid Id { get; set; }
        public Guid IngredientTagId { get; set; }
        public Guid IngredientId { get; set; }

        public virtual IngredientCategory IngredientCategory { get; set; }
        public virtual Ingredient Ingredient { get; set; }
    }
}
