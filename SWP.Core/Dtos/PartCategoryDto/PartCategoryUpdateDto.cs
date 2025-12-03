using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.PartCategoryDto
{
    /// <summary>
    /// DTO cho cập nhật Part Category
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class PartCategoryUpdateDto
    {
        /// <summary>
        /// Tên danh mục phụ tùng
        /// </summary>
        [MaxLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
        public string? PartCategoryName { get; set; }

        /// <summary>
        /// Mã danh mục phụ tùng
        /// </summary>
        [Required(ErrorMessage = "Mã danh mục không được để trống.")]
        [MaxLength(20, ErrorMessage = "Mã danh mục không được vượt quá 20 ký tự.")]
        public string PartCategoryCode { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả danh mục
        /// </summary>
        [MaxLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự.")]
        public string? PartCategoryDiscription { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        [MaxLength(50, ErrorMessage = "Số điện thoại không được vượt quá 50 ký tự.")]
        public string? PartCategoryPhone { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [MaxLength(20, ErrorMessage = "Trạng thái không được vượt quá 20 ký tự.")]
        public string? Status { get; set; }
    }
}


