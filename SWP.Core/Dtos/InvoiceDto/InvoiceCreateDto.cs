using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.InvoiceDto
{
    /// <summary>
    /// DTO cho tạo mới Invoice
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class InvoiceCreateDto
    {
        /// <summary>
        /// ID của Service Ticket (bắt buộc)
        /// </summary>
        [Required(ErrorMessage = "Service Ticket ID không được để trống.")]
        public int ServiceTicketId { get; set; }

        /// <summary>
        /// ID của Customer (bắt buộc)
        /// </summary>
        [Required(ErrorMessage = "Customer ID không được để trống.")]
        public int CustomerId { get; set; }

        /// <summary>
        /// Số tiền phụ tùng (tự động tính từ service ticket)
        /// </summary>
        public decimal? PartsAmount { get; set; }

        /// <summary>
        /// Số tiền dịch vụ (tự động tính từ service ticket)
        /// </summary>
        public decimal? GarageServiceAmount { get; set; }

        /// <summary>
        /// Thuế VAT (%)
        /// </summary>
        public decimal? TaxAmount { get; set; }

        /// <summary>
        /// Giảm giá
        /// </summary>
        public decimal? DiscountAmount { get; set; }

        /// <summary>
        /// Tổng tiền (tự động tính)
        /// </summary>
        [Required(ErrorMessage = "Tổng tiền không được để trống.")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Mã hóa đơn (nullable, có thể tự động generate)
        /// </summary>
        [MaxLength(20, ErrorMessage = "Mã hóa đơn không được vượt quá 20 ký tự.")]
        public string? InvoiceCode { get; set; }
    }
}


