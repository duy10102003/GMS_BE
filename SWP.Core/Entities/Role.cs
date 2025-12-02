using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("role")]
    public class Role
    {
        [Column("role_id")]
        [Key]
        public int RoleId { get; set; }

        [Column("role_name")]
        [Required]
        public string RoleName { get; set; } = string.Empty;

        [Column("description")]
        [MaxLength(255)]
        public string? Description { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; } = 0;
    }
}
