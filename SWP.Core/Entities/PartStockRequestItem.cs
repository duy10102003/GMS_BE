using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("part_stock_request_items")]
    public class PartStockRequestItem
    {
        [Column("part_request_item_id")]
        [Key]
        public Guid PartRequestItemId { get; set; }

        [Column("part_request_id")]
        [Required]
        public Guid PartRequestId { get; set; }

        [Column("part_id")]
        [Required]
        public Guid PartId { get; set; }

        [Column("quantity")]
        [Required]
        public int Quantity { get; set; }

        // Navigation properties
        public PartStockRequest PartStockRequest { get; set; } = null!;
        public Part Part { get; set; } = null!;
    }
}

