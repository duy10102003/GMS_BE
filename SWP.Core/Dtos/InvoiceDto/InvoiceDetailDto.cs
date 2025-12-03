using SWP.Core.Dtos.SeriveTicketDto;

namespace SWP.Core.Dtos.InvoiceDto
{
    /// <summary>
    /// DTO cho chi tiết Invoice
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class InvoiceDetailDto
    {
        public int InvoiceId { get; set; }
        public int ServiceTicketId { get; set; }
        public string? ServiceTicketCode { get; set; }
        public CustomerInfoDto Customer { get; set; } = new CustomerInfoDto();
        public VehicleInfoDto Vehicle { get; set; } = new VehicleInfoDto();
        public DateTime? IssueDate { get; set; }
        public decimal? PartsAmount { get; set; }
        public decimal? GarageServiceAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public byte? InvoiceStatus { get; set; }
        public string? InvoiceCode { get; set; }
        public List<ServiceTicketDetailItemDto> Parts { get; set; } = new List<ServiceTicketDetailItemDto>();
        public List<ServiceTicketDetailServiceDto> GarageServices { get; set; } = new List<ServiceTicketDetailServiceDto>();
    }
}

