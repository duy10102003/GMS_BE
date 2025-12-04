using System;

namespace SWP.Core.Dtos.BookingDto
{
    /// <summary>
    /// DTO chi tiet booking.
    /// </summary>
    public class BookingDetailDto
    {
        public int BookingId { get; set; }

        public DateTime BookingTime { get; set; }

        public byte? BookingStatus { get; set; }

        public string? VehicleName { get; set; }

        public string? Note { get; set; }

        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerPhone { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
