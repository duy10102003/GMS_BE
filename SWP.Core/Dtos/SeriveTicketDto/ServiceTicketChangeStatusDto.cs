using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.SeriveTicketDto
{
    /// <summary>
    /// DTO cho thay đổi status của Service Ticket
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class ServiceTicketChangeStatusDto
    {
        /// <summary>
        /// ID của người thực hiện (bắt buộc)
        /// </summary>
        [Required(ErrorMessage = "Người thực hiện không được để trống.")]
        public int ModifiedBy { get; set; }

        /// <summary>
        /// Ghi chú (optional)
        /// </summary>
        public string? Note { get; set; }
    }
}

