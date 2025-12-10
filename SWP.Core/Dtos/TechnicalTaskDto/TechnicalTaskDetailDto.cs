using SWP.Core.Dtos.SeriveTicketDto;

namespace SWP.Core.Dtos.TechnicalTaskDto
{
    /// <summary>
    /// DTO cho chi tiết Technical Task
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class TechnicalTaskDetailDto
    {
        public int TechnicalTaskId { get; set; }
        public int ServiceTicketId { get; set; }
        public string? ServiceTicketCode { get; set; }
        public string Description { get; set; } = string.Empty;
        public UserInfoDto? AssignedToTechnical { get; set; }
        public DateTime? AssignedAt { get; set; }
        public byte? TaskStatus { get; set; }
        public UserInfoDto? ConfirmedBy { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public byte? ServiceTicketStatus { get; set; }
        public CustomerInfoDto? Customer { get; set; }
        public VehicleInfoDto? Vehicle { get; set; }
        public List<ServiceTicketDetailItemDto> Parts { get; set; } = new List<ServiceTicketDetailItemDto>();
        public List<ServiceTicketDetailServiceDto> GarageServices { get; set; } = new List<ServiceTicketDetailServiceDto>();
    }
}


