using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.Ingredients
{
    /// <summary>
    /// Tags that define the category of an ingredient.
    /// </summary>
    [Table("ingredient_categories")]
    public class IngredientCategory : IEntity
    {
        public IngredientCategory()
        {
            Connections = new HashSet<IngredientCategoryConnection>();
        }

        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; }

        [MaxLength(1000)]
        [Column("description")]
        public string Description { get; set; }

        public virtual ICollection<IngredientCategoryConnection> Connections { get; set; }
    }
}
