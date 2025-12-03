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
        /// Xóa Service Ticket
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
        /// Thêm part vào Service Ticket (Staff có thể thêm)
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin part</param>
        /// <returns>ID của Service Ticket Detail vừa tạo</returns>
        Task<int> AddPartAsync(int id, ServiceTicketAddPartDto request);

        /// <summary>
        /// Thêm garage service vào Service Ticket (Staff có thể thêm)
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin garage service</param>
        /// <returns>ID của Service Ticket Detail vừa tạo</returns>
        Task<int> AddGarageServiceAsync(int id, ServiceTicketAddGarageServiceDto request);

        /// <summary>
        /// Xóa part/garage service khỏi Service Ticket
        /// </summary>
        /// <param name="serviceTicketId">ID của Service Ticket</param>
        /// <param name="serviceTicketDetailId">ID của Service Ticket Detail</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> RemoveDetailAsync(int serviceTicketId, int serviceTicketDetailId);

        /// <summary>
        /// Duyệt parts và services do Mechanic đề xuất
        /// </summary>
        /// <param name="technicalTaskId">ID của TechnicalTask</param>
        /// <param name="request">Danh sách parts và services được duyệt</param>
        /// <param name="staffId">ID của staff duyệt</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        Task<int> ApproveMechanicProposalAsync(int technicalTaskId, ServiceTicketUpdatePartsServicesDto request, int staffId);

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
    }
}




