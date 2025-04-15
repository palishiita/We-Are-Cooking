using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.UserData
{
    /// <summary>
    /// A connection between user and the tag of the ingredient that the user cannot use due to the choice or an allergy.
    /// </summary>
    [Table("user_dietary_restrictions")]
    public class UserDietaryRestriction
    {
        public Guid Id { get; set; }
        public Guid IngredientTagId { get; set; }
        public Guid UserId { get; set; }
    }
}
