namespace SWP.Core.Dtos.SeriveTicketDto
{
    /// <summary>
    /// DTO cho danh sách Service Ticket
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    public class ServiceTicketListItemDto
    {
        public Guid ServiceTicketId { get; set; }
        public string? ServiceTicketCode { get; set; }
        public Guid? BookingId { get; set; }
        public Guid VehicleId { get; set; }
        public string? VehicleName { get; set; }
        public string? VehicleLicensePlate { get; set; }
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public Guid CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte? ServiceTicketStatus { get; set; }
        public string? InitialIssue { get; set; }
        public Guid? AssignedToTechnical { get; set; }
        public string? AssignedToTechnicalName { get; set; }
    }
}

