using RecipesAPI.Entities.Ingredients;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.UserData
{
    /// <summary>
    /// A representation of an ingredient in the fridge of a user.
    /// </summary>
    [Table("user_fridge_ingredients")]
    public class UserFridgeIngredients : IEntity
    {
        public Guid Id { get; set; }
        public Guid IngredientId { get; set; }
        public Guid UserId { get; set; }

        public virtual Ingredient Ingredient { get; set; }
    }
}
