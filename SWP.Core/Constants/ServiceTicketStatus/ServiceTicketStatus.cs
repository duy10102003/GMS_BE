namespace SWP.Core.Constants.ServiceTicketStatus
{
    /// <summary>
    /// Constants cho trạng thái Service Ticket
    /// Created by: DuyLC(01/12/2025)
    /// Updated by: DuyLC(03/12/2025)
    /// </summary>
    public static class ServiceTicketStatus
    {
        /// <summary>
        /// Đang chờ technical xác nhận (0) - Khi staff tạo và assign cho technical
        /// </summary>
        public const byte PendingTechnicalConfirmation = 0;

        /// <summary>
        /// Được điều chỉnh từ technical staff (1) - Khi technical điều chỉnh part/service
        /// </summary>
        public const byte AdjustedByTechnical = 1;

        /// <summary>
        /// Đang làm (2) - Sau khi staff xác nhận điều chỉnh từ technical
        /// </summary>
        public const byte InProgress = 2;

        /// <summary>
        /// Đã hoàn thành (3) - Khi technical nhấn hoàn thành
        /// </summary>
        public const byte Completed = 3;

        /// <summary>
        /// Đã hủy (4) - Chỉ hủy được khi chưa hoàn thành
        /// </summary>
        public const byte Cancelled = 4;
    }
}





