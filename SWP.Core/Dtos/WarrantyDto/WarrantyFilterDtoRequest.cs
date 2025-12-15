using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWP.Core.Dtos.WarrantyDto
{
    public class WarrantyFilterDtoRequest
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
