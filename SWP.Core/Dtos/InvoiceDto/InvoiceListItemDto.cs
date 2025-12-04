namespace SWP.Core.Dtos.InvoiceDto
{
    /// <summary>
    /// DTO cho danh sách Invoice
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class InvoiceListItemDto
    {
        public int InvoiceId { get; set; }
        public int ServiceTicketId { get; set; }
        public string? ServiceTicketCode { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public DateTime? IssueDate { get; set; }
        public decimal? PartsAmount { get; set; }
        public decimal? GarageServiceAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public byte? InvoiceStatus { get; set; }
        public string? InvoiceCode { get; set; }
    }
}

