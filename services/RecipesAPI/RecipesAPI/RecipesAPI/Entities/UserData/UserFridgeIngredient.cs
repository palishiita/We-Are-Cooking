using RecipesAPI.Entities.Ingredients;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.UserData
{
    /// <summary>
    /// A representation of an ingredient in the fridge of a user.
    /// </summary>
    [Table("user_fridge_ingredients")]
    public class UserFridgeIngredient
    {
        [Column("ingredient_id")]
        public Guid IngredientId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("quantity")]
        public double IngredientQuantity { get; set; }

        // this needs to be moved to a different table, and be an Id as this will be also used
        [Column("unit_id")]
        public Guid UnitId { get; set; }

        //[ForeignKey(nameof(IngredientId))]
        public virtual Ingredient Ingredient { get; set; }

        public virtual Unit Unit { get; set; }

        //[ForeignKey(nameof(UserId))]
        //public virtual User User { get; set; }
    }
}
