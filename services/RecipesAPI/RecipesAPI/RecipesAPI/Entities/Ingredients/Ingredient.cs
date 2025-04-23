using System.ComponentModel.DataAnnotations;
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
            Connections = new HashSet<IngredientCategoryConnection>();
        }

        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
