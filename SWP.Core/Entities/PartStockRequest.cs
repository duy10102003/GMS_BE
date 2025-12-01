using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("part_stock_request")]
    public class PartStockRequest
    {
        [Column("part_request_id")]
        [Key]
        public Guid PartRequestId { get; set; }

        [Column("requested_by")]
        [Required]
        public Guid RequestedBy { get; set; }

        [Column("requested_date")]
        public DateTime? RequestedDate { get; set; }

        [Column("part_stock_request_status")]
        public byte? PartStockRequestStatus { get; set; }

        [Column("note")]
        public string? Note { get; set; }

        [Column("confirmed_by")]
        public Guid? ConfirmedBy { get; set; }

        [Column("confirmed_date")]
        public DateTime? ConfirmedDate { get; set; }

        // Navigation properties
        public User RequestedByUser { get; set; } = null!;
        public User? ConfirmedByUser { get; set; }
        public ICollection<PartStockRequestItem> PartStockRequestItems { get; set; } = new List<PartStockRequestItem>();
    }
}

