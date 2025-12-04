namespace SWP.Core.Dtos
{
    /// <summary>
    /// Base class cho các request có phân trang
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public class PagedRequest
    {
        /// <summary>
        /// Số trang (bắt đầu từ 1)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Số bản ghi trên mỗi trang
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Danh sách filter theo cột
        /// </summary>
        public List<ColumnFilterDto>? ColumnFilters { get; set; }

        /// <summary>
        /// Danh sách sort theo cột
        /// </summary>
        public List<ColumnSortDto>? ColumnSorts { get; set; }
    }
}

