using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("warranty")]
    public class Warranty
    {
        [Column("warranty_id")]
        [Key]
        public Guid WarrantyId { get; set; }

        [Column("service_ticket_detail_id")]
        [Required]
        public Guid ServiceTicketDetailId { get; set; }

        [Column("start_date")]
        [Required]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        [Required]
        public DateTime EndDate { get; set; }

        [Column("status")]
        public byte? Status { get; set; }

        // Navigation properties
        public ServiceTicketDetail ServiceTicketDetail { get; set; } = null!;
    }
}

