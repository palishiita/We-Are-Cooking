using RecipesAPI.Entities.Ingredients;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.UserData
{
    /// <summary>
    /// A representation of an ingredient in the fridge of a user.
    /// </summary>
    [Table("user_fridge_ingredients")]
    public class UserFridgeIngredients : IEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Key]
        [Column("ingredient_id")]
        public Guid IngredientId { get; set; }

        [Key]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(IngredientId))]
        public virtual Ingredient Ingredient { get; set; }
    }
}
