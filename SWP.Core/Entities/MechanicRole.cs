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
    [GMSTableName("mechanic_role")]
    public class MechanicRole
    {
        [Column("mechanic_role_id")]
        [Key]
        public int MechanicRoleId { get; set; }
        [Column("mechanic_role_name")]
        public string? MechanicRoleName { get; set; }
        [Column("mechanic_role_description")]
        public string? MechanicRoleDescription { get; set; }
        [Column("is_deleted")]
        public int IsDeleted { get; set; }
        public ICollection<MechanicRolePermission>? MechanicRolePermissions { get; set; }

    }
}
