using SWP.Core.Dtos;
using SWP.Core.Dtos.InvoiceDto;

namespace SWP.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho Invoice Service
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public interface IInvoiceService
    {
        /// <summary>
        /// Lấy danh sách Invoice có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Invoice</returns>
        Task<PagedResult<InvoiceListItemDto>> GetPagingAsync(InvoiceFilterDtoRequest filter);

        /// <summary>
        /// Lấy Invoice theo ID
        /// </summary>
        /// <param name="id">ID của Invoice</param>
        /// <returns>Chi tiết Invoice</returns>
        Task<InvoiceDetailDto> GetByIdAsync(int id);

        /// <summary>
        /// Tạo Invoice từ Service Ticket đã hoàn thành
        /// </summary>
        /// <param name="request">Thông tin Invoice</param>
        /// <returns>ID của Invoice vừa tạo</returns>
        Task<int> CreateFromServiceTicketAsync(InvoiceCreateDto request);
    }
}

