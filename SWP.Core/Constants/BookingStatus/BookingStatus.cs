namespace SWP.Core.Constants.BookingStatus
{
    /// <summary>
    /// Constants trang thai Booking
    /// </summary>
    public static class BookingStatus
    {
        /// <summary>
        /// Chờ staff duyệt
        /// </summary>
        public const byte Pending = 0;

        /// <summary>
        /// Đã duyệt
        /// </summary>
        public const byte Confirmed = 1;

        /// <summary>
        /// Staff từ chối
        /// </summary>
        public const byte Declined = 2;

        /// <summary>
        /// Khách hủy
        /// </summary>
        public const byte Cancelled = 3;

        /// <summary>
        /// Đã hoàn thành
        /// </summary>
        public const byte Completed = 4;

        /// <summary>
        /// Khách không đến
        /// </summary>
        public const byte NoShow = 5;
    }
}
