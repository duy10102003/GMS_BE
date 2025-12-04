using SWP.Core.Dtos;

namespace SWP.Core.Dtos.PartDto
{
    /// <summary>
    /// DTO cho filter và phân trang Part
    /// Created by: DuyLC(02/12/2025)
    /// </summary>
    public class PartFilterDtoRequest
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


