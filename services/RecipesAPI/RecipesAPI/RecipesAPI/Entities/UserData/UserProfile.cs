using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecipesAPI.Entities.Reviews;

namespace RecipesAPI.Entities.UserData
{
    [Table("user_profiles")]
    public class UserProfile
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("photo_url_id")]
        public Guid PhotoUrlId { get; set; }

        [Column("description")]
        [MaxLength(2000)]
        public String Description { get; set; }

        public virtual User User { get; set; }

        public virtual PhotoUrl PhotoUrl { get; set; }
    }
}
