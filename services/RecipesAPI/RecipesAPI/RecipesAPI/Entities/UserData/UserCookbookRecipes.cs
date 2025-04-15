using RecipesAPI.Entities.Recipes;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.UserData
{
    /// <summary>
    /// Saved recipes are added to the user cookbook.
    /// </summary>
    [Table("user_cookbook_recipes")]
    public class UserCookbookRecipes
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public Guid UserId { get; set; }
        public bool IsFavorite { get; set; }

        public virtual Recipe Recipe { get; set; }
    }
}
