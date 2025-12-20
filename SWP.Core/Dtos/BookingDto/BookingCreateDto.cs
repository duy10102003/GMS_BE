using System;

namespace SWP.Core.Dtos.BookingDto
{
    /// <summary>
    /// Request tạo booking mới từ customer.
    /// </summary>
    public class BookingCreateDto
    {
        public int CustomerId { get; set; }

        public DateTime BookingTime { get; set; }

        public string? VehicleName { get; set; }
        
        public string? Reason { get; set; }

        public string? Note { get; set; }
    }
}
