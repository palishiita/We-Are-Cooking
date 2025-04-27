using RecipesAPI.Entities.UserData;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.Ingredients
{
    /// <summary>
    /// Tags that define the category of an ingredient.
    /// </summary>
    [Table("ingredients_categories")]
    public class IngredientCategory
    {
        public IngredientCategory()
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
        public virtual ICollection<UserDietaryRestriction> UserDietaryRestrictions { get; set; }
    }
}
