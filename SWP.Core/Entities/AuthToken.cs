using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("auth_token")]
    public class AuthToken
    {
        [Column("token_id")]
        [Key]
        public int TokenId { get; set; }

        [Column("token")]
        [Required]
        [MaxLength(128)]
        public string Token { get; set; } = string.Empty;

        [Column("type")]
        [Required]
        [MaxLength(30)]
        public string Type { get; set; } = string.Empty;

        [Column("user_id")]
        [Required]
        public int UserId { get; set; }

        [Column("created_at")]
        [Required]
        public DateTime CreatedAt { get; set; }

        [Column("expires_at")]
        [Required]
        public DateTime ExpiresAt { get; set; }

        [Column("used")]
        [Required]
        public bool Used { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
    }
}
