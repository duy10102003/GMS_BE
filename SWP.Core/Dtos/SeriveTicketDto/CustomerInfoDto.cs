using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.SeriveTicketDto
{
    /// <summary>
    /// DTO cho thông tin customer
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    public class CustomerInfoDto
    {
        /// <summary>
        /// ID của customer (dùng cho response, nullable cho request)
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [MaxLength(100, ErrorMessage = "Tên khách hàng không được vượt quá 100 ký tự.")]
        public string? CustomerName { get; set; }

        /// <summary>
        /// Số điện thoại (bắt buộc)
        /// </summary>
        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [MaxLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
        public string CustomerPhone { get; set; } = string.Empty;

        /// <summary>
        /// Email
        /// </summary>
        [MaxLength(50, ErrorMessage = "Email không được vượt quá 50 ký tự.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string? CustomerEmail { get; set; }

        /// <summary>
        /// ID của user (nếu khách hàng có tài khoản)
        /// </summary>
        public int? UserId { get; set; }
    }
}




