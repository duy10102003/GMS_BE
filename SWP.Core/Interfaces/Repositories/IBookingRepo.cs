using SWP.Core.Dtos;
using SWP.Core.Dtos.BookingDto;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Repositories
{
    public interface IBookingRepo : IBaseRepo<Booking>
    {
        Task<PagedResult<BookingListItemDto>> GetPagingAsync(BookingFilterDtoRequest filter);
        Task<BookingDetailDto?> GetDetailAsync(int id);
    }
}
