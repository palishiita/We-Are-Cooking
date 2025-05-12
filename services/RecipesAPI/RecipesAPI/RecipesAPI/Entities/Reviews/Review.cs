using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecipesAPI.Entities.Recipes;
using RecipesAPI.Entities.UserData;

namespace RecipesAPI.Entities.Reviews
{
    [Table("review")]
    public class Review
    {
        public Review()
        {
            ReviewPhotos = new HashSet<ReviewPhoto>();
        }

        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("recipe_id")]
        public Guid RecipeId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("rating")]
        public float Rating { get; set; }

        [Column("description")]
        [MaxLength(2000)]
        public string? Description { get; set; }

        [Column("has_photos")]
        public bool HasPhotos { get; set; }

        public virtual Recipe Recipe { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<ReviewPhoto> ReviewPhotos { get; set; } = new HashSet<ReviewPhoto>();
    }
}
