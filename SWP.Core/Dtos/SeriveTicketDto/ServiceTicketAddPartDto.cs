using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.SeriveTicketDto
{
    /// <summary>
    /// DTO cho thêm part vào Service Ticket
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    public class ServiceTicketAddPartDto
    {
        /// <summary>
        /// ID của part
        /// </summary>
        [Required(ErrorMessage = "Part ID không được để trống.")]
        public Guid PartId { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        [Required(ErrorMessage = "Số lượng không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; }
    }
}


