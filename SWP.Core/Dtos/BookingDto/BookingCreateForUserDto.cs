using System;

namespace SWP.Core.Dtos.BookingDto
{
    /// <summary>
    /// Request tao booking cho nguoi dung da dang nhap.
    /// </summary>
    public class BookingCreateForUserDto
    {
        // public int UserId { get; set; }

        // public DateTime BookingTime { get; set; }

        // public string? VehicleName { get; set; }
        
        // public string? Reason { get; set; }

        // public string? Note { get; set; }
        public int UserId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        // ✅ THÊM DÒNG NÀY
        public string CustomerPhone { get; set; } = string.Empty;

        public string? CustomerEmail { get; set; }

        public DateTime BookingTime { get; set; }

        public string? VehicleName { get; set; }

        public string? Reason { get; set; }

        public string? Note { get; set; }        
    }
}
