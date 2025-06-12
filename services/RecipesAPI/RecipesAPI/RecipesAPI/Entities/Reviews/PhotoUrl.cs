using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesAPI.Entities.Reviews
{
    [Table("photo_urls")]
    public class PhotoUrl
    {
        public PhotoUrl() 
        { 
            ReviewPhotoAssociated = new HashSet<ReviewPhoto>();
        }

        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("photo_url")]
        [MaxLength(512)]
        public string Url { get; set; }

        public virtual ICollection<ReviewPhoto> ReviewPhotoAssociated { get; set; }
    }
}
