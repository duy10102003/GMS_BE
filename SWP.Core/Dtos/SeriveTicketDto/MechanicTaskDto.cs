namespace SWP.Core.Dtos.SeriveTicketDto
{
    /// <summary>
    /// DTO cho task của Mechanic
    /// Created by: DuyLC(02/12/2025)
    /// </summary>
    public class MechanicTaskDto
    {
        public int TechnicalTaskId { get; set; }
        public int ServiceTicketId { get; set; }
        public string? ServiceTicketCode { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime? AssignedAt { get; set; }
        public byte? TaskStatus { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public ServiceTicketInfoDto ServiceTicket { get; set; } = new ServiceTicketInfoDto();
        public List<ServiceTicketDetailItemDto> Parts { get; set; } = new List<ServiceTicketDetailItemDto>();
        public List<ServiceTicketDetailServiceDto> GarageServices { get; set; } = new List<ServiceTicketDetailServiceDto>();
    }

    /// <summary>
    /// DTO cho thông tin service ticket trong task
    /// </summary>
    public class ServiceTicketInfoDto
    {
        public int ServiceTicketId { get; set; }
        public string? ServiceTicketCode { get; set; }
        public byte? ServiceTicketStatus { get; set; }
        public string? InitialIssue { get; set; }
        public VehicleInfoDto Vehicle { get; set; } = new VehicleInfoDto();
        public CustomerInfoDto Customer { get; set; } = new CustomerInfoDto();
    }
}

