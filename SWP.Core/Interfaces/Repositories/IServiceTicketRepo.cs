using SWP.Core.Dtos;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Entities;

namespace SWP.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface cho Service Ticket Repository
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    public interface IServiceTicketRepo : IBaseRepo<ServiceTicket>
    {
        /// <summary>
        /// Lấy danh sách Service Ticket có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Service Ticket</returns>
        Task<PagedResult<ServiceTicketListItemDto>> GetPagingAsync(ServiceTicketFilterDtoRequest filter);

        /// <summary>
        /// Lấy chi tiết Service Ticket theo ID
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <returns>Chi tiết Service Ticket</returns>
        Task<ServiceTicketDetailDto?> GetDetailAsync(int id);

        /// <summary>
        /// Kiểm tra mã Service Ticket đã tồn tại chưa
        /// </summary>
        /// <param name="code">Mã Service Ticket</param>
        /// <param name="excludeId">ID cần loại trừ (khi update)</param>
        /// <returns>True nếu đã tồn tại</returns>
        Task<bool> CheckCodeExistsAsync(string code, int? excludeId = null);

        /// <summary>
        /// Lấy danh sách tasks của Mechanic có phân trang
        /// </summary>
        /// <param name="mechanicId">ID của Mechanic</param>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách tasks</returns>
        Task<PagedResult<MechanicTaskDto>> GetMechanicTasksAsync(int mechanicId, ServiceTicketFilterDtoRequest filter);

        /// <summary>
        /// Lấy chi tiết task của Mechanic
        /// </summary>
        /// <param name="technicalTaskId">ID của TechnicalTask</param>
        /// <param name="mechanicId">ID của Mechanic</param>
        /// <returns>Chi tiết task</returns>
        Task<MechanicTaskDto?> GetMechanicTaskDetailAsync(int technicalTaskId, int mechanicId);
    }
}




