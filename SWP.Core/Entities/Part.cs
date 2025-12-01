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
        public Guid PartId { get; set; }

        [Column("part_name")]
        [Required]
        public string PartName { get; set; } = string.Empty;

        [Column("part_price", TypeName = "decimal(24,2)")]
        [Required]
        public decimal PartPrice { get; set; }

        [Column("part_code")]
        [Required]
        [MaxLength(20)]
        public string PartCode { get; set; } = string.Empty;

        [Column("number_month_warranty")]
        [Required]
        public int NumberMonthWarranty { get; set; }

        [Column("part_quantity")]
        [Required]
        public int PartQuantity { get; set; }

        [Column("part_unit")]
        [Required]
        [MaxLength(20)]
        public string PartUnit { get; set; } = string.Empty;

        [Column("created_date")]
        [Required]
        public DateTime CreatedDate { get; set; }

        [Column("created_by")]
        [Required]
        public Guid CreatedBy { get; set; }

        [Column("modified_date")]
        [Required]
        public DateTime ModifiedDate { get; set; }

        [Column("modified_by")]
        [Required]
        public Guid ModifiedBy { get; set; }

        [Column("part_stock")]
        [Required]
        public int PartStock { get; set; }

        // Navigation properties
        public User CreatedByUser { get; set; } = null!;
        public User ModifiedByUser { get; set; } = null!;
        public ICollection<ServiceTicketDetail> ServiceTicketDetails { get; set; } = new List<ServiceTicketDetail>();
        public ICollection<PartStockRequestItem> PartStockRequestItems { get; set; } = new List<PartStockRequestItem>();
    }
}

