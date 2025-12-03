namespace SWP.Core.Dtos.GarageServiceDto
{
    /// <summary>
    /// DTO cho search Garage Service
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class GarageServiceSearchRequest
    {
        /// <summary>
        /// Từ khóa tìm kiếm (tên dịch vụ)
        /// </summary>
        public string? SearchKeyword { get; set; }

        /// <summary>
        /// Số lượng kết quả tối đa (mặc định 50)
        /// </summary>
        public int Limit { get; set; } = 50;
    }
}

