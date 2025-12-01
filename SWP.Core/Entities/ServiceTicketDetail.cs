using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("service_ticket_detail")]
    public class ServiceTicketDetail
    {
        [Column("service_ticket_detail_id")]
        [Key]
        public Guid ServiceTicketDetailId { get; set; }

        [Column("part_id")]
        [Required]
        public Guid PartId { get; set; }

        [Column("service_ticket_id")]
        public Guid? ServiceTicketId { get; set; }

        [Column("quantity")]
        public int? Quantity { get; set; }

        // Navigation properties
        public Part Part { get; set; } = null!;
        public ServiceTicket? ServiceTicket { get; set; }
        public ICollection<Warranty> Warranties { get; set; } = new List<Warranty>();
    }
}

