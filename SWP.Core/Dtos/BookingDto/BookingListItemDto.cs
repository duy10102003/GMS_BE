using System;

namespace SWP.Core.Dtos.BookingDto
{
    /// <summary>
    /// DTO tra ve cho danh sach booking.
    /// </summary>
    public class BookingListItemDto
    {
        public int BookingId { get; set; }

        public DateTime BookingTime { get; set; }

        public byte? BookingStatus { get; set; }

        public string? VehicleName { get; set; }
        
        public string? Reason { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerPhone { get; set; }
    }
}
