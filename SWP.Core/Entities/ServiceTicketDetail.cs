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
        public int ServiceTicketDetailId { get; set; }

        [Column("part_id")]
        public int? PartId { get; set; }

        [Column("garage_service_id")]
        public int? GarageServiceId { get; set; }

        [Column("service_ticket_id")]
        public int? ServiceTicketId { get; set; }

        [Column("quantity")]
        public int? Quantity { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; } = 0;

        // Navigation properties
        public Part? Part { get; set; }
        public GarageService? GarageService { get; set; }
        public ServiceTicket? ServiceTicket { get; set; }
        public ICollection<Warranty> Warranties { get; set; } = new List<Warranty>();
    }
}
