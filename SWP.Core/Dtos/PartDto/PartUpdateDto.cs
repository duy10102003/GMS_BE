using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.PartDto
{
    /// <summary>
    /// DTO cho cập nhật Part
    /// Created by: DuyLC(02/12/2025)
    /// </summary>
    public class PartUpdateDto
    {
        /// <summary>
        /// Tên phụ tùng
        /// </summary>
        [Required(ErrorMessage = "Tên phụ tùng không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên phụ tùng không được vượt quá 100 ký tự.")]
        public string PartName { get; set; } = string.Empty;

        /// <summary>
        /// Mã phụ tùng
        /// </summary>
        [Required(ErrorMessage = "Mã phụ tùng không được để trống.")]
        [MaxLength(20, ErrorMessage = "Mã phụ tùng không được vượt quá 20 ký tự.")]
        public string PartCode { get; set; } = string.Empty;

        /// <summary>
        /// Số lượng tồn kho
        /// </summary>
        [Required(ErrorMessage = "Số lượng tồn kho không được để trống.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0.")]
        public int PartQuantity { get; set; }

        /// <summary>
        /// Đơn vị tính
        /// </summary>
        [Required(ErrorMessage = "Đơn vị tính không được để trống.")]
        [MaxLength(20, ErrorMessage = "Đơn vị tính không được vượt quá 20 ký tự.")]
        public string PartUnit { get; set; } = string.Empty;

        /// <summary>
        /// ID danh mục phụ tùng
        /// </summary>
        [Required(ErrorMessage = "ID danh mục phụ tùng không được để trống.")]
        public int PartCategoryId { get; set; }

        /// <summary>
        /// Giá phụ tùng
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Giá phụ tùng phải lớn hơn hoặc bằng 0.")]
        public decimal? PartPrice { get; set; }

        /// <summary>
        /// Thời gian bảo hành (tháng)
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Thời gian bảo hành phải lớn hơn hoặc bằng 0.")]
        public int? WarrantyMonth { get; set; }
    }
}

