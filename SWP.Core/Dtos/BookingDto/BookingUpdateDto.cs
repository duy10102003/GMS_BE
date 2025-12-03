using System;

namespace SWP.Core.Dtos.BookingDto
{
    /// <summary>
    /// Request cập nhật thông tin booking.
    /// </summary>
    public class BookingUpdateDto
    {
        public DateTime BookingTime { get; set; }

        public string VehicleName { get; set; } = string.Empty;

        public string? Note { get; set; }

        /// <summary>
        /// Cập nhật trạng thái booking (Pending/Confirmed/Declined/Cancelled/Completed/NoShow).
        /// </summary>
        public byte BookingStatus { get; set; }
    }
}
