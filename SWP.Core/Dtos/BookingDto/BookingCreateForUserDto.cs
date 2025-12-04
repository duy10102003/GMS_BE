using System;

namespace SWP.Core.Dtos.BookingDto
{
    /// <summary>
    /// Request tao booking cho nguoi dung da dang nhap.
    /// </summary>
    public class BookingCreateForUserDto
    {
        public int UserId { get; set; }

        public DateTime BookingTime { get; set; }

        public string? VehicleName { get; set; }

        public string? Note { get; set; }
    }
}
