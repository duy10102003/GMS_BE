namespace SWP.Core.Dtos.VehicleDto
{
    /// <summary>
    /// DTO cho search Vehicle
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class VehicleSearchRequest
    {
        /// <summary>
        /// ID của Customer (để lọc xe của customer đó)
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// Từ khóa tìm kiếm (tên xe, biển số)
        /// </summary>
        public string? SearchKeyword { get; set; }

        /// <summary>
        /// Số lượng kết quả tối đa (mặc định 50)
        /// </summary>
        public int Limit { get; set; } = 50;
    }
}

