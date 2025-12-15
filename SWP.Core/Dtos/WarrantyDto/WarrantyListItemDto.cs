using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWP.Core.Dtos.WarrantyDto
{
    public class WarrantyListItemDto
    {
        public int WarrantyId { get; set; }
        public int ServiceTicketDetailId { get; set; }

        public string PartName { get; set; } = string.Empty;
        public string PartCode { get; set; } = string.Empty;

        public string VehicleLicensePlate { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public byte? Status { get; set; }
    }
}
