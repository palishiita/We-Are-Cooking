using RecipesAPI.Entities.Ingredients;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.UserData
{
    /// <summary>
    /// A connection between user and the tag of the ingredient that the user cannot use due to the choice or an allergy.
    /// </summary>
    [Table("user_dietary_restrictions")]
    public class UserDietaryRestriction
    {
        [Key]
        [Column("ingredient_category_id")]
        public Guid IngredientCategoryId { get; set; }

        [Key]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(IngredientCategoryId))]
        public virtual IngredientCategory IngredientCategory { get; set; }
    }
}
