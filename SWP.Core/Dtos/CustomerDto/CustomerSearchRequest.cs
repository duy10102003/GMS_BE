namespace SWP.Core.Dtos.CustomerDto
{
    /// <summary>
    /// DTO cho search Customer
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class CustomerSearchRequest
    {
        /// <summary>
        /// Từ khóa tìm kiếm (tên, số điện thoại, email)
        /// </summary>
        public string? SearchKeyword { get; set; }

        /// <summary>
        /// Số lượng kết quả tối đa (mặc định 50)
        /// </summary>
        public int Limit { get; set; } = 50;
    }
}

