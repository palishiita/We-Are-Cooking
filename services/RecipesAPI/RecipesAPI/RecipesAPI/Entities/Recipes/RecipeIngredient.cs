using RecipesAPI.Entities.Ingredients;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.Recipes
{
    /// <summary>
    /// Connection between the recipe and an ingredient.
    /// </summary>
    [Table("recipe_ingredients")]
    public class RecipeIngredient
    {
        [Key]
        [Column("recipe_id")]
        [Required]
        public Guid RecipeId { get; set; }

        [Key]
        [Column("ingredient_id")]
        [Required]
        public Guid IngredientId { get; set; }


        [ForeignKey(nameof(RecipeId))]
        public virtual Recipe Recipe { get; set; }

        [ForeignKey(nameof(IngredientId))]
        public virtual Ingredient Ingredient { get; set; }
    }
}
