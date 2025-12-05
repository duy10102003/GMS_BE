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

        [Column("part_quantity")]
        [Required]
        public int PartQuantity { get; set; }

        [Column("part_unit")]
        [Required]
        [MaxLength(20)]
        public string PartUnit { get; set; } = string.Empty;

        [Column("part_category_id")]
        [Required]
        public int PartCategoryId { get; set; }

        [Column("part_price", TypeName = "decimal(24,2)")]
        public decimal? PartPrice { get; set; }

        [Column("warranty_month")]
        public int? WarrantyMonth { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; } = 0;

        //
        public PartCategory PartCategory { get; set; } = null!;
        public ICollection<ServiceTicketDetail> ServiceTicketDetails { get; set; } = new List<ServiceTicketDetail>();
        public ICollection<PartStockOutItem> PartStockOutItems { get; set; } = new List<PartStockOutItem>();
        public ICollection<PartPriceHistory> PartPriceHistories { get; set; } = new List<PartPriceHistory>();
    }
}
