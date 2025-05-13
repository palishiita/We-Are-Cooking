using RecipesAPI.Entities.UserData;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.Recipes
{
    /// <summary>
    /// A recipe.
    /// </summary>
    [Table("recipes")]
    public class Recipe
    {
        public Recipe()
        {
            Ingredients = new HashSet<RecipeIngredient>();
        }

        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("posting_user_id")]
        public Guid PostingUserId { get; set; }

        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; }

        [MaxLength(2000)]
        [Column("description")]
        public string Description { get; set; }

        public virtual ICollection<RecipeIngredient> Ingredients { get; set; }
        public virtual ICollection<UserCookbookRecipe> UserCookbooks { get; set; }
        //public virtual User PostingUser { get; set; }
    }
}
