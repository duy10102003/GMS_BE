using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.SeriveTicketDto
{
    /// <summary>
    /// DTO cho assign Service Ticket cho technical staff
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    public class ServiceTicketAssignDto
    {
        /// <summary>
        /// ID của technical staff được assign
        /// </summary>
        [Required(ErrorMessage = "Technical staff ID không được để trống.")]
        public Guid AssignedToTechnical { get; set; }

        /// <summary>
        /// Mô tả công việc
        /// </summary>
        [Required(ErrorMessage = "Mô tả công việc không được để trống.")]
        [MaxLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự.")]
        public string Description { get; set; } = string.Empty;
    }
}


