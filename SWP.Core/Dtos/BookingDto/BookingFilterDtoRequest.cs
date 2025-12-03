using System.Collections.Generic;
using SWP.Core.Dtos;

namespace SWP.Core.Dtos.BookingDto
{
    /// <summary>
    /// DTO filter + paging cho danh sach booking.
    /// </summary>
    public class BookingFilterDtoRequest
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public byte? Status { get; set; }

        public int? CustomerId { get; set; }
        
        public List<ColumnFilterDto>? ColumnFilters { get; set; }

        public List<ColumnSortDto>? ColumnSorts { get; set; }
    }
}
