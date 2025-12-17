using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.BookingDto
{
    /// <summary>
    /// DTO cho phép staff thay đổi trạng thái booking.
    /// </summary>
    public class BookingChangeStatusDto
    {
        /// <summary>
        /// Trạng thái mới (Pending/Confirmed/Declined/Cancelled/Completed/NoShow).
        /// </summary>
        [Required(ErrorMessage = "BookingStatus không được để trống.")]
        public byte BookingStatus { get; set; }

        /// <summary>
        /// Lý do thay đổi trạng thái (tùy chọn).
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// Ghi chú cho lần duyệt (tùy chọn).
        /// </summary>
        public string? Note { get; set; }
    }
}
