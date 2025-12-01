using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("user_token")]
    public class UserToken
    {
        [Column("user_token_id")]
        [Key]
        public Guid UserTokenId { get; set; }

        [Column("user_id")]
        [Required]
        public Guid UserId { get; set; }

        [Column("access_token")]
        [MaxLength(512)]
        public string? AccessToken { get; set; }

        [Column("refresh_token")]
        [MaxLength(512)]
        public string? RefreshToken { get; set; }

        [Column("type")]
        [MaxLength(255)]
        public string? Type { get; set; }

        [Column("status")]
        [MaxLength(255)]
        public string? Status { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
    }
}

