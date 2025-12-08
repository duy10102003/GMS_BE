using SWP.Core.GMSAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWP.Core.Entities
{
    [GMSTableName("mechanic_role_permission")]
    public class MechanicRolePermission
    {
        [Column("mechanic_role_permission_id")]
        [Key]
        public int MechanicRolePermissionId { get; set; }
        [Column("mechanic_role_id")]
        [Required]
        public int MechanicRoleId { get; set; }
        [Column("user_id")]
        [Required]
        public int UserId { get; set; }
        [Column("year_exp")]
        public int YearExp { get; set; }
        [Column("is_deleted")]
        public int IsDeleted { get; set; }
        public MechanicRole? MechanicRole { get; set; }
        public User? User { get; set; }
    }
}
