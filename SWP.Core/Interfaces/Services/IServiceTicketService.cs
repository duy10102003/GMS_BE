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
        Task<ServiceTicketDetailDto> GetByIdAsync(Guid id);

        /// <summary>
        /// Tạo mới Service Ticket
        /// </summary>
        /// <param name="request">Thông tin Service Ticket mới</param>
        /// <returns>ID của Service Ticket vừa tạo</returns>
        Task<Guid> CreateAsync(ServiceTicketCreateDto request);

        /// <summary>
        /// Cập nhật Service Ticket
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> UpdateAsync(Guid id, ServiceTicketUpdateDto request);

        /// <summary>
        /// Xóa Service Ticket
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> DeleteAsync(Guid id);

        /// <summary>
        /// Assign Service Ticket cho technical staff
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin assign</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> AssignToTechnicalAsync(Guid id, ServiceTicketAssignDto request);

        /// <summary>
        /// Technical staff nhận task
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="technicalStaffId">ID của technical staff</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> AcceptTaskAsync(Guid id, Guid technicalStaffId);

        /// <summary>
        /// Thêm part vào Service Ticket
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin part</param>
        /// <returns>ID của Service Ticket Detail vừa tạo</returns>
        Task<Guid> AddPartAsync(Guid id, ServiceTicketAddPartDto request);

        /// <summary>
        /// Xóa part khỏi Service Ticket
        /// </summary>
        /// <param name="serviceTicketId">ID của Service Ticket</param>
        /// <param name="serviceTicketDetailId">ID của Service Ticket Detail</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> RemovePartAsync(Guid serviceTicketId, Guid serviceTicketDetailId);
    }
}

