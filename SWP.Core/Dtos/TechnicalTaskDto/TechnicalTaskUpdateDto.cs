using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.TechnicalTaskDto
{
    /// <summary>
    /// DTO cho cập nhật Technical Task
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class TechnicalTaskUpdateDto
    {
        /// <summary>
        /// Mô tả công việc
        /// </summary>
        [MaxLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự.")]
        public string? Description { get; set; }

        /// <summary>
        /// Trạng thái task
        /// </summary>
        public byte? TaskStatus { get; set; }
    }
}


