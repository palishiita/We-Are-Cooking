using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecipesAPI.Entities.UserData;

namespace RecipesAPI.Entities.Reviews
{
    [Table("photo_urls")]
    public class PhotoUrl
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("photo_url")]
        [MaxLength(512)]
        public string Url { get; set; }

        public virtual ReviewPhoto ReviewPhoto { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
