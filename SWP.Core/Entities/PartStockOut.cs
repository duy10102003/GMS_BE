using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("part_stock_out")]
    public class PartStockOut
    {
        [Column("part_stock_out_id")]
        [Key]
        public int PartStockOutId { get; set; }

        [Column("part_stock_out_code")]
        [Required]
        [MaxLength(255)]
        public string PartStockOutCode { get; set; } = string.Empty;

        [Column("requested_by")]
        [Required]
        public int RequestedBy { get; set; }

        [Column("requested_date")]
        public DateTime? RequestedDate { get; set; }

        [Column("part_stock_stock_out_status")]
        [MaxLength(50)]
        public string? PartStockStockOutStatus { get; set; }

        [Column("part_stock_stock_out_type")]
        [MaxLength(50)]
        public string? PartStockStockOutType { get; set; }

        [Column("note")]
        public string? Note { get; set; }

        [Column("confirmed_by")]
        public int? ConfirmedBy { get; set; }

        [Column("confirmed_date")]
        public DateTime? ConfirmedDate { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; } = 0;

        // Navigation properties
        public User RequestedByUser { get; set; } = null!;
        public User? ConfirmedByUser { get; set; }
        public ICollection<PartStockOutItem> PartStockOutItems { get; set; } = new List<PartStockOutItem>();
    }
}

