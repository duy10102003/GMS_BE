using SWP.Core.Dtos;

namespace SWP.Core.Dtos.InvoiceDto
{
    /// <summary>
    /// DTO cho filter Invoice
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class InvoiceFilterDtoRequest : PagedRequest
    {
        /// <summary>
        /// ID của Customer
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// Trạng thái hóa đơn
        /// </summary>
        public byte? InvoiceStatus { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? ToDate { get; set; }
    }
}


