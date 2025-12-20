using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.SeriveTicketDto
{
    /// <summary>
    /// DTO cho thêm garage service vào Service Ticket
    /// Created by: DuyLC(02/12/2025)
    /// </summary>
    public class ServiceTicketAddGarageServiceDto
    {
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



