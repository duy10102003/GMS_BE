using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("inventory")]
    public class Inventory
    {
        [Column("part_id")]
        [Key]
        public int PartId { get; set; }

        [Column("inventory_price", TypeName = "decimal(24,2)")]
        public decimal InventoryPrice { get; set; } = 0;

        [Column("warranty_month")]
        [Required]
        public int WarrantyMonth { get; set; }

        [Column("inventory_quantity")]
        [Required]
        public int InventoryQuantity { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; } = 0;

        // Navigation properties
        public Part Part { get; set; } = null!;
    }
}

