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

        ///// <summary>
        ///// ID của User (liên kết qua Customer.UserId)
        ///// Dùng cho trường hợp lấy danh sách hóa đơn theo tài khoản người dùng.
        ///// </summary>
        //public int? UserId { get; set; }

        /// <summary>
        /// Trạng thái hóa đơn
        /// </summary>
        public byte? InvoiceStatus { get; set; }

        /// <summary>
        /// Key để search
        /// </summary>
        public string? KeyWord { get; set; }

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


