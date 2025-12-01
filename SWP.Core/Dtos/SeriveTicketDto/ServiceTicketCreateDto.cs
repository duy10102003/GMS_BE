using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.SeriveTicketDto
{
    /// <summary>
    /// DTO cho tạo mới Service Ticket
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    public class ServiceTicketCreateDto
    {
        /// <summary>
        /// ID của booking (nullable)
        /// </summary>
        public Guid? BookingId { get; set; }

        /// <summary>
        /// ID của customer có sẵn (nullable - nếu có thì dùng customer này, không thì tạo mới từ CustomerInfo)
        /// </summary>
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// Thông tin customer mới (nullable - chỉ dùng khi CustomerId không có)
        /// </summary>
        public CustomerInfoDto? CustomerInfo { get; set; }

        /// <summary>
        /// ID của vehicle có sẵn (nullable - nếu có thì dùng vehicle này, không thì tạo mới từ VehicleInfo)
        /// </summary>
        public Guid? VehicleId { get; set; }

        /// <summary>
        /// Thông tin vehicle mới (nullable - chỉ dùng khi VehicleId không có)
        /// </summary>
        public VehicleInfoDto? VehicleInfo { get; set; }

        /// <summary>
        /// ID của người tạo (bắt buộc)
        /// </summary>
        [Required(ErrorMessage = "Người tạo không được để trống.")]
        public Guid CreatedBy { get; set; }

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
        /// ID của technical staff được assign (nullable, có thể assign sau)
        /// </summary>
        public Guid? AssignedToTechnical { get; set; }

        /// <summary>
        /// Mô tả công việc khi assign (nullable, chỉ dùng khi có AssignedToTechnical)
        /// </summary>
        [MaxLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự.")]
        public string? AssignDescription { get; set; }
    }
}

