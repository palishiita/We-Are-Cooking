using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.Reviews
{
    [Table("review_photos")]
    public class ReviewPhoto
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("review_id")]
        public Guid ReviewId { get; set; }

        [Required]
        [Column("photo_url")]
        public Guid PhotoUrl { get; set; }

        public virtual Review Review { get; set; }
        public virtual PhotoUrl Photo { get; set; }
    }
}
