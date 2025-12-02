using System.ComponentModel.DataAnnotations;

namespace SWP.Core.Dtos.SeriveTicketDto
{
    /// <summary>
    /// DTO cho thông tin vehicle
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    public class VehicleInfoDto
    {
        /// <summary>
        /// Tên xe
        /// </summary>
        [Required(ErrorMessage = "Tên xe không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên xe không được vượt quá 100 ký tự.")]
        public string VehicleName { get; set; } = string.Empty;

        /// <summary>
        /// Biển số xe
        /// </summary>
        [Required(ErrorMessage = "Biển số xe không được để trống.")]
        [MaxLength(255, ErrorMessage = "Biển số xe không được vượt quá 255 ký tự.")]
        public string VehicleLicensePlate { get; set; } = string.Empty;

        /// <summary>
        /// Số km hiện tại
        /// </summary>
        public int? CurrentKm { get; set; }

        /// <summary>
        /// Hãng xe
        /// </summary>
        [MaxLength(255, ErrorMessage = "Hãng xe không được vượt quá 255 ký tự.")]
        public string? Make { get; set; }

        /// <summary>
        /// Model xe
        /// </summary>
        [MaxLength(255, ErrorMessage = "Model xe không được vượt quá 255 ký tự.")]
        public string? Model { get; set; }

        /// <summary>
        /// ID của customer (nullable)
        /// </summary>
        public Guid? CustomerId { get; set; }
    }
}


