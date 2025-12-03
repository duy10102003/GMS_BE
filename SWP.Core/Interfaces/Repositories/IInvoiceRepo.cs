using SWP.Core.Dtos;
using SWP.Core.Dtos.InvoiceDto;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface cho Invoice Repository
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public interface IInvoiceRepo : IBaseRepo<Invoice>
    {
        /// <summary>
        /// Lấy danh sách Invoice có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Invoice</returns>
        Task<PagedResult<InvoiceListItemDto>> GetPagingAsync(InvoiceFilterDtoRequest filter);

        /// <summary>
        /// Lấy chi tiết Invoice theo ID
        /// </summary>
        /// <param name="id">ID của Invoice</param>
        /// <returns>Chi tiết Invoice</returns>
        Task<InvoiceDetailDto?> GetDetailAsync(int id);

        /// <summary>
        /// Lấy Invoice theo Service Ticket ID
        /// </summary>
        /// <param name="serviceTicketId">ID của Service Ticket</param>
        /// <returns>Invoice</returns>
        Task<Invoice?> GetByServiceTicketIdAsync(int serviceTicketId);
    }
}

