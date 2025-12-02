using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("part_stock_in_items")]
    public class PartStockInItem
    {
        [Column("part_stock_in_item_id")]
        [Key]
        public int PartStockInItemId { get; set; }

        [Column("part_stock_in_id")]
        [Required]
        public int PartStockInId { get; set; }

        [Column("part_id")]
        [Required]
        public int PartId { get; set; }

        [Column("quantity")]
        [Required]
        public int Quantity { get; set; }

        [Column("stock_in_price", TypeName = "decimal(24,2)")]
        public decimal? StockInPrice { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; } = 0;

        // Navigation properties
        public PartStockIn PartStockIn { get; set; } = null!;
        public Part Part { get; set; } = null!;
    }
}

