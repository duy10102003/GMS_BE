using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.SeriveTicketDto
{
    /// <summary>
    /// DTO cho cập nhật parts và services trong service ticket (Mechanic gửi cho Staff)
    /// Created by: DuyLC(02/12/2025)
    /// </summary>
    public class ServiceTicketUpdatePartsServicesDto
    {
        /// <summary>
        /// Danh sách parts
        /// </summary>
        public List<ServiceTicketPartDto> Parts { get; set; } = new List<ServiceTicketPartDto>();

        /// <summary>
        /// Danh sách garage services
        /// </summary>
        public List<ServiceTicketServiceDto> GarageServices { get; set; } = new List<ServiceTicketServiceDto>();
    }

    /// <summary>
    /// DTO cho part trong service ticket
    /// </summary>
    public class ServiceTicketPartDto
    {
        /// <summary>
        /// ID của service ticket detail (nếu update) hoặc null (nếu thêm mới)
        /// </summary>
        public int? ServiceTicketDetailId { get; set; }

        /// <summary>
        /// ID của part
        /// </summary>
        [Required(ErrorMessage = "Part ID không được để trống.")]
        public int PartId { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        [Required(ErrorMessage = "Số lượng không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; }
    }

    /// <summary>
    /// DTO cho garage service trong service ticket
    /// </summary>
    public class ServiceTicketServiceDto
    {
        /// <summary>
        /// ID của service ticket detail (nếu update) hoặc null (nếu thêm mới)
        /// </summary>
        public int? ServiceTicketDetailId { get; set; }

        /// <summary>
        /// ID của garage service
        /// </summary>
        [Required(ErrorMessage = "Garage Service ID không được để trống.")]
        public int GarageServiceId { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        [Required(ErrorMessage = "Số lượng không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; }
    }
}

