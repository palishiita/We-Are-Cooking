using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.Reviews
{
    [Table("review_photos")]
    public class ReviewPhoto
    {
        [Required]
        [Column("review_id")]
        public Guid ReviewId { get; set; }

        [Required]
        [Column("photo_id")]
        public Guid PhotoId { get; set; }

        public virtual Review Review { get; set; }
        public virtual PhotoUrl PhotoUrl { get; set; }
    }
}
