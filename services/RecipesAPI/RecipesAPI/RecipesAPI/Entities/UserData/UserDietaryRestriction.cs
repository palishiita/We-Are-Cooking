using RecipesAPI.Entities.Ingredients;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.UserData
{
    /// <summary>
    /// A connection between user and the tag of the ingredient that the user cannot use due to the choice or an allergy.
    /// </summary>
    [Table("user_dietary_restrictions")]
    public class UserDietaryRestriction : IEntity
    {
        public Guid Id { get; set; }
        public Guid IngredientCategoryId { get; set; }
        public Guid UserId { get; set; }

        public virtual IngredientCategory IngredientCategory { get; set; }
    }
}
