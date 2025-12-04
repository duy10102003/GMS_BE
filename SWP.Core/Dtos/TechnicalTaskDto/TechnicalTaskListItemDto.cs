namespace SWP.Core.Dtos.TechnicalTaskDto
{
    /// <summary>
    /// DTO cho danh sách Technical Task
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class TechnicalTaskListItemDto
    {
        public int TechnicalTaskId { get; set; }
        public int ServiceTicketId { get; set; }
        public string? ServiceTicketCode { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? AssignedToTechnical { get; set; }
        public string? AssignedToTechnicalName { get; set; }
        public DateTime? AssignedAt { get; set; }
        public byte? TaskStatus { get; set; }
        public int? ConfirmedBy { get; set; }
        public string? ConfirmedByName { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public byte? ServiceTicketStatus { get; set; }
        public string? CustomerName { get; set; }
        public string? VehicleName { get; set; }
        public string? VehicleLicensePlate { get; set; }
    }
}

