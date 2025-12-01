using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("otp_verification")]
    public class OtpVerification
    {
        [Column("otp_verification_id")]
        [Key]
        public Guid OtpVerificationId { get; set; }

        [Column("user_id")]
        [Required]
        public Guid UserId { get; set; }

        [Column("email")]
        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Column("otp_code")]
        [Required]
        [MaxLength(6)]
        public string OtpCode { get; set; } = string.Empty;

        [Column("action")]
        [Required]
        [MaxLength(20)]
        public string Action { get; set; } = string.Empty;

        [Column("status")]
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        [Column("verified")]
        [Required]
        public bool Verified { get; set; }

        [Column("created_at")]
        [Required]
        public DateTime CreatedAt { get; set; }

        [Column("expire_at")]
        [Required]
        public DateTime ExpireAt { get; set; }

        [Column("ip_address")]
        [MaxLength(64)]
        public string? IpAddress { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
    }
}

