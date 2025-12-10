using SWP.Core.Dtos;
using SWP.Core.Dtos.SeriveTicketDto;

namespace SWP.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho Service Ticket Service
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    public interface IServiceTicketService
    {
        #region Staff Operations

        /// <summary>
        /// Lấy danh sách Service Ticket có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Service Ticket</returns>
        Task<PagedResult<ServiceTicketListItemDto>> GetPagingAsync(ServiceTicketFilterDtoRequest filter);

        /// <summary>
        /// Lấy Service Ticket theo ID
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <returns>Chi tiết Service Ticket</returns>
        Task<ServiceTicketDetailDto> GetByIdAsync(int id);

        /// <summary>
        /// Tạo mới Service Ticket
        /// </summary>
        /// <param name="request">Thông tin Service Ticket mới</param>
        /// <returns>ID của Service Ticket vừa tạo</returns>
        Task<int> CreateAsync(ServiceTicketCreateDto request);

        /// <summary>
        /// Cập nhật Service Ticket (thông tin khách hàng, xe, status)
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> UpdateAsync(int id, ServiceTicketUpdateDto request);

        /// <summary>
        /// Xóa Service Ticket (chỉ khi chưa hoàn thành, tự động rollback part quantity)
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> DeleteAsync(int id);

        /// <summary>
        /// Assign Service Ticket cho technical staff (tạo TechnicalTask)
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin assign</param>
        /// <returns>ID của TechnicalTask vừa tạo</returns>
        Task<int> AssignToTechnicalAsync(int id, ServiceTicketAssignDto request);

        /// <summary>
        /// Chuyển status sang PendingTechnicalConfirmation (0)
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin thay đổi</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> ChangeToPendingTechnicalConfirmationAsync(int id, ServiceTicketChangeStatusDto request);

        /// <summary>
        /// Chuyển status sang AdjustedByTechnical (1)
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin thay đổi</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> ChangeToAdjustedByTechnicalAsync(int id, ServiceTicketChangeStatusDto request);

        /// <summary>
        /// Chuyển status sang InProgress (2)
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin thay đổi</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> ChangeToInProgressAsync(int id, ServiceTicketChangeStatusDto request);

        /// <summary>
        /// Chuyển status sang Completed (3)
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin thay đổi</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> ChangeToCompletedAsync(int id, ServiceTicketChangeStatusDto request);

        /// <summary>
        /// Chuyển status sang Cancelled (4) - Tự động rollback part quantity
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin thay đổi</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> ChangeToCancelledAsync(int id, ServiceTicketChangeStatusDto request);

        #endregion

        #region Mechanic Operations

        /// <summary>
        /// Lấy danh sách tasks của Mechanic
        /// </summary>
        /// <param name="mechanicId">ID của Mechanic</param>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách tasks</returns>
        Task<PagedResult<MechanicTaskDto>> GetMyTasksAsync(int mechanicId, ServiceTicketFilterDtoRequest filter);

        /// <summary>
        /// Lấy chi tiết task của Mechanic
        /// </summary>
        /// <param name="technicalTaskId">ID của TechnicalTask</param>
        /// <param name="mechanicId">ID của Mechanic</param>
        /// <returns>Chi tiết task</returns>
        Task<MechanicTaskDto> GetMyTaskDetailAsync(int technicalTaskId, int mechanicId);

        /// <summary>
        /// Mechanic đề xuất parts và services (gửi cho Staff duyệt)
        /// </summary>
        /// <param name="technicalTaskId">ID của TechnicalTask</param>
        /// <param name="request">Danh sách parts và services</param>
        /// <param name="mechanicId">ID của Mechanic</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> ProposePartsServicesAsync(int technicalTaskId, ServiceTicketUpdatePartsServicesDto request, int mechanicId);

        /// <summary>
        /// Mechanic bắt đầu làm task (duyệt và bắt đầu)
        /// </summary>
        /// <param name="technicalTaskId">ID của TechnicalTask</param>
        /// <param name="mechanicId">ID của Mechanic</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> StartTaskAsync(int technicalTaskId, int mechanicId);

        /// <summary>
        /// Mechanic confirm task đã hoàn thành
        /// </summary>
        /// <param name="technicalTaskId">ID của TechnicalTask</param>
        /// <param name="mechanicId">ID của Mechanic</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> ConfirmTaskAsync(int technicalTaskId, int mechanicId);

        #endregion

        #region Customer Operations 
        Task<int> ChangeToCustomerConfirmationAsync(int id);

        /// <summary>
        /// Lấy danh sách Service Ticket có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Service Ticket</returns>
        Task<PagedResult<ServiceTicketListItemDto>> GetPagingServiceTicketForCustomerAsync(int id,ServiceTicketFilterDtoRequest filter);

        #endregion
    }
}




