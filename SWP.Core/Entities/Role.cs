using SWP.Core.GMSAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWP.Core.Entities
{
    [GMSTableName("role")]
    public class Role
    {
        [Column("role_id")]
        [Key]
        public Guid RoleId { get; set; }

        [Column("role_name")]
        public string RoleName { get; set; }

        [Column("description")]
        [MaxLength(255)]
        public string? Description { get; set; }
    }
}
