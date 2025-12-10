using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.GarageServiceDto
{
    /// <summary>
    /// DTO cho tạo mới Garage Service
    /// Created by: DuyLC(02/12/2025)
    /// </summary>
    public class GarageServiceCreateDto
    {
        /// <summary>
        /// Tên dịch vụ
        /// </summary>
        [Required(ErrorMessage = "Tên dịch vụ không được để trống.")]
        [MaxLength(255, ErrorMessage = "Tên dịch vụ không được vượt quá 255 ký tự.")]
        public string GarageServiceName { get; set; } = string.Empty;

        /// <summary>
        /// Giá dịch vụ
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Giá dịch vụ phải lớn hơn hoặc bằng 0.")]
        public decimal? GarageServicePrice { get; set; }
    }
}



