using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("part_stock_in")]
    public class PartStockIn
    {
        [Column("part_stock_in_id")]
        [Key]
        public int PartStockInId { get; set; }

        [Column("part_stock_in_code")]
        [Required]
        [MaxLength(255)]
        public string PartStockInCode { get; set; } = string.Empty;

        [Column("supplier_id")]
        [Required]
        public int SupplierId { get; set; }

        [Column("delivery_personel")]
        [MaxLength(255)]
        public string? DeliveryPersonel { get; set; }

        [Column("note")]
        public string? Note { get; set; }

        [Column("stock_in_date")]
        public DateTime? StockInDate { get; set; }

        [Column("by_stocker")]
        [Required]
        public int ByStocker { get; set; }

        [Column("total_cost", TypeName = "decimal(24,2)")]
        public decimal? TotalCost { get; set; }

        [Column("vat")]
        public int? Vat { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; } = 0;

        // Navigation properties
        public Supplier Supplier { get; set; } = null!;
        public User ByStockerUser { get; set; } = null!;
        public ICollection<PartStockInItem> PartStockInItems { get; set; } = new List<PartStockInItem>();
    }
}




