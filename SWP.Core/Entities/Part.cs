using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("part")]
    public class Part
    {
        [Column("part_id")]
        [Key]
        public int PartId { get; set; }

        [Column("part_name")]
        [Required]
        [MaxLength(100)]
        public string PartName { get; set; } = string.Empty;

        [Column("part_code")]
        [Required]
        [MaxLength(20)]
        public string PartCode { get; set; } = string.Empty;

        [Column("part_stock")]
        [Required]
        public int PartStock { get; set; }

        [Column("part_unit")]
        [Required]
        [MaxLength(20)]
        public string PartUnit { get; set; } = string.Empty;

        [Column("supplier_id")]
        [Required]
        public int SupplierId { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; } = 0;

        // Navigation properties
        public Supplier Supplier { get; set; } = null!;
        public Inventory? Inventory { get; set; }
        public ICollection<ServiceTicketDetail> ServiceTicketDetails { get; set; } = new List<ServiceTicketDetail>();
        public ICollection<PartStockInItem> PartStockInItems { get; set; } = new List<PartStockInItem>();
        public ICollection<PartStockOutItem> PartStockOutItems { get; set; } = new List<PartStockOutItem>();
        public ICollection<PartPriceHistory> PartPriceHistories { get; set; } = new List<PartPriceHistory>();
    }
}
