using SWP.Core.Dtos;
using SWP.Core.Dtos.TechnicalTaskDto;

namespace SWP.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho Technical Task Service
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    public interface ITechnicalTaskService
    {
        /// <summary>
        /// Lấy danh sách Technical Task có phân trang (cho technical staff)
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Technical Task</returns>
        Task<PagedResult<TechnicalTaskListItemDto>> GetPagingAsync(TechnicalTaskFilterDtoRequest filter);

        /// <summary>
        /// Lấy Technical Task theo ID
        /// </summary>
        /// <param name="id">ID của Technical Task</param>
        /// <returns>Chi tiết Technical Task</returns>
        Task<TechnicalTaskDetailDto> GetByIdAsync(int id);

        /// <summary>
        /// Technical staff điều chỉnh Parts và Services trong Service Ticket
        /// </summary>
        /// <param name="technicalTaskId">ID của Technical Task</param>
        /// <param name="request">Danh sách Parts và Services mới</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> AdjustPartsServicesAsync(int technicalTaskId, TechnicalTaskAdjustPartsServicesDto request);

        /// <summary>
        /// Staff xác nhận điều chỉnh từ technical (chuyển status sang InProgress)
        /// </summary>
        /// <param name="technicalTaskId">ID của Technical Task</param>
        /// <param name="confirmedBy">ID của staff xác nhận</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> ConfirmAdjustmentAsync(int technicalTaskId, int confirmedBy);

        /// <summary>
        /// Technical staff hoàn thành công việc (chuyển status sang Completed)
        /// </summary>
        /// <param name="technicalTaskId">ID của Technical Task</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> CompleteTaskAsync(int technicalTaskId);
    }
}


