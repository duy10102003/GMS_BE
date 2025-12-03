using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("supplier")]
    public class Supplier
    {
        [Column("supplier_id")]
        [Key]
        public int SupplierId { get; set; }

        [Column("supplier_name")]
        [Required]
        [MaxLength(100)]
        public string SupplierName { get; set; } = string.Empty;

        [Column("supplier_code")]
        [Required]
        [MaxLength(20)]
        public string SupplierCode { get; set; } = string.Empty;

        [Column("supplier_addr")]
        [Required]
        [MaxLength(255)]
        public string SupplierAddr { get; set; } = string.Empty;

        [Column("supplier_phone")]
        [Required]
        [MaxLength(50)]
        public string SupplierPhone { get; set; } = string.Empty;

        [Column("tax_identification_number")]
        [Required]
        [MaxLength(255)]
        public string TaxIdentificationNumber { get; set; } = string.Empty;

        [Column("total_part_cost", TypeName = "decimal(30,2)")]
        public decimal? TotalPartCost { get; set; }

        [Column("supplier_type")]
        [MaxLength(50)]
        public string? SupplierType { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; } = 0;

        // Navigation properties
        public ICollection<Part> Parts { get; set; } = new List<Part>();
        public ICollection<PartStockIn> PartStockIns { get; set; } = new List<PartStockIn>();
    }
}

