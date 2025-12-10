using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.SeriveTicketDto
{
    /// <summary>
    /// DTO cho tạo mới Service Ticket với Parts và Services
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class ServiceTicketCreateWithPartsServicesDto
    {
        /// <summary>
        /// ID của booking (nullable)
        /// </summary>
        public int? BookingId { get; set; }

        /// <summary>
        /// ID của customer có sẵn (nullable - nếu có thì dùng customer này, không thì tạo mới từ CustomerInfo)
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// Thông tin customer mới (nullable - chỉ dùng khi CustomerId không có)
        /// </summary>
        public CustomerInfoDto? CustomerInfo { get; set; }

        /// <summary>
        /// ID của vehicle có sẵn (nullable - nếu có thì dùng vehicle này, không thì tạo mới từ VehicleInfo)
        /// Lưu ý: Vehicle phải thuộc về Customer được chọn
        /// </summary>
        public int? VehicleId { get; set; }

        /// <summary>
        /// Thông tin vehicle mới (nullable - chỉ dùng khi VehicleId không có)
        /// </summary>
        public VehicleInfoDto? VehicleInfo { get; set; }

        /// <summary>
        /// ID của người tạo (bắt buộc)
        /// </summary>
        [Required(ErrorMessage = "Người tạo không được để trống.")]
        public int CreatedBy { get; set; }

        /// <summary>
        /// Vấn đề ban đầu
        /// </summary>
        public string? InitialIssue { get; set; }

        /// <summary>
        /// Mã service ticket (nullable, có thể tự động generate)
        /// </summary>
        [MaxLength(20, ErrorMessage = "Mã service ticket không được vượt quá 20 ký tự.")]
        public string? ServiceTicketCode { get; set; }

        /// <summary>
        /// ID của technical staff được assign (bắt buộc khi tạo)
        /// </summary>
        [Required(ErrorMessage = "Technical staff không được để trống.")]
        public int AssignedToTechnical { get; set; }

        /// <summary>
        /// Mô tả công việc khi assign
        /// </summary>
        [Required(ErrorMessage = "Mô tả công việc không được để trống.")]
        [MaxLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự.")]
        public string AssignDescription { get; set; } = string.Empty;

        /// <summary>
        /// Danh sách Parts (nullable, có thể thêm sau)
        /// </summary>
        public List<ServiceTicketAddPartDto>? Parts { get; set; }

        /// <summary>
        /// Danh sách Garage Services (nullable, có thể thêm sau)
        /// </summary>
        public List<ServiceTicketAddGarageServiceDto>? GarageServices { get; set; }
    }
}


