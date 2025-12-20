using SWP.Core.Dtos;
using SWP.Core.Dtos.TechnicalTaskDto;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface cho Technical Task Repository
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public interface ITechnicalTaskRepo : IBaseRepo<TechnicalTask>
    {
        /// <summary>
        /// Lấy danh sách Technical Task có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Technical Task</returns>
        Task<PagedResult<TechnicalTaskListItemDto>> GetPagingAsync(TechnicalTaskFilterDtoRequest filter);

        /// <summary>
        /// Lấy chi tiết Technical Task theo ID
        /// </summary>
        /// <param name="id">ID của Technical Task</param>
        /// <returns>Chi tiết Technical Task</returns>
        Task<TechnicalTaskDetailDto?> GetDetailAsync(int id);

        /// <summary>
        /// Lấy Technical Task theo Service Ticket ID
        /// </summary>
        /// <param name="serviceTicketId">ID của Service Ticket</param>
        /// <returns>Technical Task</returns>
        Task<TechnicalTask?> GetByServiceTicketIdAsync(int serviceTicketId);
    }
}


