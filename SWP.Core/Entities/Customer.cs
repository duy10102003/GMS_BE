using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("customer")]
    public class Customer
    {
        [Column("customer_id")]
        [Key]
        public int CustomerId { get; set; }

        [Column("customer_name")]
        [MaxLength(100)]
        public string? CustomerName { get; set; }

        [Column("customer_phone")]
        [Required]
        [MaxLength(20)]
        public string CustomerPhone { get; set; } = string.Empty;

        [Column("customer_email")]
        [MaxLength(50)]
        public string? CustomerEmail { get; set; }

        [Column("user_id")]
        public int? UserId { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; } = 0;

        // Navigation properties
        public User? User { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
