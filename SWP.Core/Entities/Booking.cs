using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("bookings")]
    public class Booking
    {
        [Column("booking_id")]
        [Key]
        public Guid BookingId { get; set; }

        [Column("customer_id")]
        [Required]
        public Guid CustomerId { get; set; }

        [Column("booking_time")]
        [Required]
        public DateTime BookingTime { get; set; }

        [Column("vehicle_name")]
        public string? VehicleName { get; set; }

        [Column("booking_status")]
        public byte? BookingStatus { get; set; }

        [Column("note")]
        public string? Note { get; set; }

        [Column("created_date")]
        public DateTime? CreatedDate { get; set; }

        [Column("modified_date")]
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public Customer Customer { get; set; } = null!;
        public ICollection<ServiceTicket> ServiceTickets { get; set; } = new List<ServiceTicket>();
    }
}

