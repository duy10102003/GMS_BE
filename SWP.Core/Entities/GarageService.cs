using SWP.Core.GMSAttribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP.Core.Entities
{
    [GMSTableName("garage_service")]
    public class GarageService
    {
        [Column("garage_service_id")]
        [Key]
        public int GarageServiceId { get; set; }

        [Column("garage_service_name")]
        [MaxLength(255)]
        public string? GarageServiceName { get; set; }

        [Column("garage_service_price", TypeName = "decimal(24,2)")]
        public decimal? GarageServicePrice { get; set; }

        [Column("is_deleted")]
        public int IsDeleted { get; set; } = 0;

        // Navigation properties
        public ICollection<ServiceTicketDetail> ServiceTicketDetails { get; set; } = new List<ServiceTicketDetail>();
    }
}





