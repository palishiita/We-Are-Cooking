using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.Ingredients
{
    /// <summary>
    /// A connection between ingredient and its tag.
    /// </summary>
    [Table("ingredient_categories_connections")]
    public class IngredientCategoryConnection
    {
        [Key]
        [Required]
        [Column("category_id")]
        public Guid CategoryId { get; set; }

        [Key]
        [Required]
        [Column("ingredient_id")]
        public Guid IngredientId { get; set; }


        [ForeignKey(nameof(CategoryId))]
        public virtual IngredientCategory IngredientCategory { get; set; }

        [ForeignKey(nameof(IngredientId))]
        public virtual Ingredient Ingredient { get; set; }
    }
}
