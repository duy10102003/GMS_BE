namespace SWP.Core.Dtos.PartDto
{
    /// <summary>
    /// DTO cho search Part
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class PartSearchRequest
    {
        /// <summary>
        /// Từ khóa tìm kiếm (tên, mã part)
        /// </summary>
        public string? SearchKeyword { get; set; }

        /// <summary>
        /// Số lượng kết quả tối đa (mặc định 50)
        /// </summary>
        public int Limit { get; set; } = 50;
    }
}


