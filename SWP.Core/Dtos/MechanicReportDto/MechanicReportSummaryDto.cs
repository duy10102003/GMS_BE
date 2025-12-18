namespace SWP.Core.Dtos.MechanicReportDto
{
    /// <summary>
    /// DTO tổng hợp báo cáo cho mechanic
    /// </summary>
    public class MechanicReportSummaryDto
    {
        public int MechanicId { get; set; }
        public string? MechanicName { get; set; }

        public int TotalTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int AdjustedTasks { get; set; }
        public int CompletedTasks { get; set; }

        public int TotalServiceTickets { get; set; }
        public int TotalVehicles { get; set; }
        public int TotalCustomers { get; set; }
    }
}

