using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("vehicle")]
    public class Vehicle
    {
        [Column("vehicle_id")]
        [Key]
        public int VehicleId { get; set; }

        [Column("vehicle_name")]
        [Required]
        [MaxLength(100)]
        public string VehicleName { get; set; } = string.Empty;

        [Column("vehicle_license_plate")]
        [Required]
        [MaxLength(255)]
        public string VehicleLicensePlate { get; set; } = string.Empty;

        [Column("current_km")]
        public int? CurrentKm { get; set; }

        [Column("customer_id")]
        public int? CustomerId { get; set; }

        [Column("make")]
        [MaxLength(255)]
        public string? Make { get; set; }

        [Column("model")]
        [MaxLength(255)]
        public string? Model { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; } = 0;

        // Navigation properties
        public Customer? Customer { get; set; }
        public ICollection<ServiceTicket> ServiceTickets { get; set; } = new List<ServiceTicket>();
    }
}
