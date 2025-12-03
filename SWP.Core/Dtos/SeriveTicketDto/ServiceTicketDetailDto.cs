namespace SWP.Core.Dtos.SeriveTicketDto
{
    /// <summary>
    /// DTO cho chi tiết Service Ticket
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    public class ServiceTicketDetailDto
    {
        public int ServiceTicketId { get; set; }
        public string? ServiceTicketCode { get; set; }
        public int? BookingId { get; set; }
        public VehicleInfoDto Vehicle { get; set; } = new VehicleInfoDto();
        public CustomerInfoDto Customer { get; set; } = new CustomerInfoDto();
        public UserInfoDto CreatedByUser { get; set; } = new UserInfoDto();
        public UserInfoDto? ModifiedByUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte? ServiceTicketStatus { get; set; }
        public string? InitialIssue { get; set; }
        public List<ServiceTicketDetailItemDto> Parts { get; set; } = new List<ServiceTicketDetailItemDto>();
        public List<ServiceTicketDetailServiceDto> GarageServices { get; set; } = new List<ServiceTicketDetailServiceDto>();
        public List<TechnicalTaskDto> TechnicalTasks { get; set; } = new List<TechnicalTaskDto>();
    }



    /// <summary>
    /// DTO cho thông tin user
    /// </summary>
    public class UserInfoDto
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }

    /// <summary>
    /// DTO cho chi tiết part trong service ticket
    /// </summary>
    public class ServiceTicketDetailItemDto
    {
        public int ServiceTicketDetailId { get; set; }
        public PartInfoDto Part { get; set; } = new PartInfoDto();
        public int Quantity { get; set; }
    }

    /// <summary>
    /// DTO cho thông tin part
    /// </summary>
    public class PartInfoDto
    {
        public int PartId { get; set; }
        public string PartName { get; set; } = string.Empty;
        public string PartCode { get; set; } = string.Empty;
        public decimal? InventoryPrice { get; set; }
        public int PartStock { get; set; }
        public string PartUnit { get; set; } = string.Empty;
        public SupplierInfoDto? Supplier { get; set; }
    }

    /// <summary>
    /// DTO cho thông tin supplier
    /// </summary>
    public class SupplierInfoDto
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string SupplierCode { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho garage service trong service ticket
    /// </summary>
    public class ServiceTicketDetailServiceDto
    {
        public int ServiceTicketDetailId { get; set; }
        public GarageServiceInfoDto GarageService { get; set; } = new GarageServiceInfoDto();
        public int Quantity { get; set; }
    }

    /// <summary>
    /// DTO cho thông tin garage service
    /// </summary>
    public class GarageServiceInfoDto
    {
        public int GarageServiceId { get; set; }
        public string? GarageServiceName { get; set; }
        public decimal? GarageServicePrice { get; set; }
    }

    /// <summary>
    /// DTO cho technical task
    /// </summary>
    public class TechnicalTaskDto
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
    }
}




