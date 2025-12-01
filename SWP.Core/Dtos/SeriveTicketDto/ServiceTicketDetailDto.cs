namespace SWP.Core.Dtos.SeriveTicketDto
{
    /// <summary>
    /// DTO cho chi tiết Service Ticket
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    public class ServiceTicketDetailDto
    {
        public Guid ServiceTicketId { get; set; }
        public string? ServiceTicketCode { get; set; }
        public Guid? BookingId { get; set; }
        public Guid VehicleId { get; set; }
        public string? VehicleName { get; set; }
        public string? VehicleLicensePlate { get; set; }
        public string? VehicleMake { get; set; }
        public string? VehicleModel { get; set; }
        public int? VehicleCurrentKm { get; set; }
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public Guid CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte? ServiceTicketStatus { get; set; }
        public string? InitialIssue { get; set; }
        public List<ServiceTicketDetailItemDto> Parts { get; set; } = new List<ServiceTicketDetailItemDto>();
        public List<TechnicalTaskDto> TechnicalTasks { get; set; } = new List<TechnicalTaskDto>();
    }

    /// <summary>
    /// DTO cho chi tiết part trong service ticket
    /// </summary>
    public class ServiceTicketDetailItemDto
    {
        public Guid ServiceTicketDetailId { get; set; }
        public Guid PartId { get; set; }
        public string PartName { get; set; } = string.Empty;
        public string PartCode { get; set; } = string.Empty;
        public decimal PartPrice { get; set; }
        public int Quantity { get; set; }
        public string PartUnit { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho technical task
    /// </summary>
    public class TechnicalTaskDto
    {
        public Guid TechnicalTaskId { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid? AssignedToTechnical { get; set; }
        public string? AssignedToTechnicalName { get; set; }
        public DateTime? AssignedAt { get; set; }
        public byte? TaskStatus { get; set; }
        public Guid? ConfirmedBy { get; set; }
        public string? ConfirmedByName { get; set; }
        public DateTime? ConfirmedAt { get; set; }
    }
}

