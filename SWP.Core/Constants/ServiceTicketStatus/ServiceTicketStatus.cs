namespace SWP.Core.Constants.ServiceTicketStatus
{
    /// <summary>
    /// Constants cho trạng thái Service Ticket
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    public static class ServiceTicketStatus
    {
        /// <summary>
        /// Mới tạo - Chờ assign
        /// </summary>
        public const byte Pending = 0;

        /// <summary>
        /// Đã assign cho technical staff
        /// </summary>
        public const byte Assigned = 1;

        /// <summary>
        /// Technical staff đã nhận task
        /// </summary>
        public const byte InProgress = 2;

        /// <summary>
        /// Đã hoàn thành
        /// </summary>
        public const byte Completed = 3;

        /// <summary>
        /// Đã hủy
        /// </summary>
        public const byte Cancelled = 4;
    }
}




