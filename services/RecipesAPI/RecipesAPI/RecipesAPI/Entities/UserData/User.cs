using RecipesAPI.Entities.Recipes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.UserData
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("first_name")]
        public string FistName { get; set; }

        [Column("second_name")]
        public string SecondName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("email_address")]
        public string EmailAddress { get; set; }

        [Column("password_hash")]
        public string PasswordHash { get; set; }

        public virtual ICollection<Recipe> UserRecipes { get; set; }

        public virtual ICollection<UserCookbookRecipe> CookbookRecipes { get; set; }
        public virtual ICollection<UserDietaryRestriction> DietaryRestrictions { get; set; }
        public virtual ICollection<UserFridgeIngredient> FridgeIngredients { get; set; }
    }
}
