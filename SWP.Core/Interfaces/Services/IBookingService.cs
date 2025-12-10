using SWP.Core.Dtos;
using SWP.Core.Dtos.BookingDto;

namespace SWP.Core.Interfaces.Services
{
    public interface IBookingService
    {
        Task<PagedResult<BookingListItemDto>> GetPagingAsync(BookingFilterDtoRequest filter);
        Task<BookingDetailDto> GetByIdAsync(int id);
        Task<int> CreateForGuestAsync(BookingCreateGuestDto request);
        Task<int> CreateForUserAsync(BookingCreateForUserDto request);
        Task<int> UpdateAsync(int id, BookingCreateDto request);
        Task<int> DeleteAsync(int id);
        Task<int> ChangeStatusAsync(int id, BookingChangeStatusDto request);
    }
}
