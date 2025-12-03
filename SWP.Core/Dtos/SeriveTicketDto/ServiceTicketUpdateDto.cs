using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.SeriveTicketDto
{
    /// <summary>
    /// DTO cho cập nhật Service Ticket
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    public class ServiceTicketUpdateDto
    {
        /// <summary>
        /// ID của booking (nullable)
        /// </summary>
        public int? BookingId { get; set; }

        /// <summary>
        /// Thông tin vehicle để cập nhật (nullable - chỉ cập nhật thông tin, không đổi vehicle_id)
        /// </summary>
        public VehicleInfoDto? VehicleInfo { get; set; }

        public CustomerInfoDto? CustomerInfo { get; set; }

        /// <summary>
        /// ID của người cập nhật (bắt buộc)
        /// </summary>
        [Required(ErrorMessage = "Người cập nhật không được để trống.")]
        public int ModifiedBy { get; set; }

        /// <summary>
        /// Vấn đề ban đầu
        /// </summary>
        public string? InitialIssue { get; set; }

        /// <summary>
        /// Mã service ticket
        /// </summary>
        [MaxLength(20, ErrorMessage = "Mã service ticket không được vượt quá 20 ký tự.")]
        public string? ServiceTicketCode { get; set; }

        /// <summary>
        /// Danh sách Parts mới (nullable - nếu có thì thay thế toàn bộ)
        /// </summary>
        public List<ServiceTicketAddPartDto>? Parts { get; set; }

        /// <summary>
        /// Danh sách Garage Service IDs mới (nullable - nếu có thì thay thế toàn bộ)
        /// </summary>
        public List<int>? GarageServiceIds { get; set; }
    }
}

