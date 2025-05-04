using RecipesAPI.Entities.Recipes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.UserData
{
    /// <summary>
    /// Saved recipes are added to the user cookbook.
    /// </summary>
    [Table("user_cookbook_recipes")]
    public class UserCookbookRecipe
    {
        [Column("recipe_id")]
        public Guid RecipeId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("is_favorite")]
        public bool IsFavorite { get; set; }


        //[ForeignKey(nameof(RecipeId))]
        public virtual Recipe Recipe { get; set; }

        //[ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}
