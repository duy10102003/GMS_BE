using System;

namespace SWP.Core.Dtos.BookingDto
{
    /// <summary>
    /// Request tao booking cho khach vang lai (guest).
    /// </summary>
    public class BookingCreateGuestDto
    {
        public string? CustomerName { get; set; }

        public string CustomerPhone { get; set; } = string.Empty;

        public string? CustomerEmail { get; set; }

        public DateTime BookingTime { get; set; }

        public string? VehicleName { get; set; }

        public string? Note { get; set; }
    }
}
