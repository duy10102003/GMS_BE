using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("part_stock_out_items")]
    public class PartStockOutItem
    {
        [Column("part_stock_out_item_id")]
        [Key]
        public int PartStockOutItemId { get; set; }

        [Column("part_stock_out_id")]
        [Required]
        public int PartStockOutId { get; set; }

        [Column("part_id")]
        [Required]
        public int PartId { get; set; }

        [Column("quantity")]
        [Required]
        public int Quantity { get; set; }

        [Column("stock_out_price", TypeName = "decimal(24,2)")]
        public decimal? StockOutPrice { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; } = 0;

        // Navigation properties
        public PartStockOut PartStockOut { get; set; } = null!;
        public Part Part { get; set; } = null!;
    }
}

