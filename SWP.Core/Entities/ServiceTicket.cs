using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("service_ticket")]
    public class ServiceTicket
    {
        [Column("service_ticket_id")]
        [Key]
        public Guid ServiceTicketId { get; set; }

        [Column("booking_id")]
        public Guid? BookingId { get; set; }

        [Column("vehicle_id")]
        [Required]
        public Guid VehicleId { get; set; }

        [Column("created_by")]
        [Required]
        public Guid CreatedBy { get; set; }

        [Column("created_date")]
        public DateTime? CreatedDate { get; set; }

        [Column("modified_by")]
        public Guid? ModifiedBy { get; set; }

        [Column("modified_date")]
        public DateTime? ModifiedDate { get; set; }

        [Column("service_ticket_status")]
        public byte? ServiceTicketStatus { get; set; }

        [Column("initial_issue")]
        public string? InitialIssue { get; set; }

        [Column("service_ticket_code")]
        public string? ServiceTicketCode { get; set; }

        // Navigation properties
        public Booking? Booking { get; set; }
        public Vehicle Vehicle { get; set; } = null!;
        public User CreatedByUser { get; set; } = null!;
        public User? ModifiedByUser { get; set; }
        public ICollection<ServiceTicketDetail> ServiceTicketDetails { get; set; } = new List<ServiceTicketDetail>();
        public ICollection<TechnicalTask> TechnicalTasks { get; set; } = new List<TechnicalTask>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}

