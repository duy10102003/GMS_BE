using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("audit_log")]
    public class AuditLog
    {
        [Column("log_id")]
        [Key]
        public Guid LogId { get; set; }

        [Column("action")]
        [MaxLength(255)]
        public string? Action { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("user_id")]
        public Guid? UserId { get; set; }

        // Navigation properties
        public User? User { get; set; }
    }
}

