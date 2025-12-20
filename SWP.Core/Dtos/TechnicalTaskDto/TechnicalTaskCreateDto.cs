using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.TechnicalTaskDto
{
    /// <summary>
    /// DTO cho tạo mới Technical Task
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class TechnicalTaskCreateDto
    {
        /// <summary>
        /// ID của Service Ticket
        /// </summary>
        [Required(ErrorMessage = "Service Ticket ID không được để trống.")]
        public int ServiceTicketId { get; set; }

        /// <summary>
        /// Mô tả công việc
        /// </summary>
        [Required(ErrorMessage = "Mô tả công việc không được để trống.")]
        [MaxLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự.")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// ID của technical staff được assign
        /// </summary>
        [Required(ErrorMessage = "Technical staff không được để trống.")]
        public int AssignedToTechnical { get; set; }
    }
}


