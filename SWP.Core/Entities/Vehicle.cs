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
        public Guid VehicleId { get; set; }

        [Column("vehicle_name")]
        [Required]
        public string VehicleName { get; set; } = string.Empty;

        [Column("vehicle_license_plate")]
        [Required]
        public string VehicleLicensePlate { get; set; } = string.Empty;

        [Column("current_km")]
        public int? CurrentKm { get; set; }

        [Column("customer_id")]
        public Guid? CustomerId { get; set; }

        [Column("make")]
        public string? Make { get; set; }

        [Column("model")]
        public string? Model { get; set; }

        // Navigation properties
        public Customer? Customer { get; set; }
        public ICollection<ServiceTicket> ServiceTickets { get; set; } = new List<ServiceTicket>();
    }
}

