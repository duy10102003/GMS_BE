using Microsoft.AspNetCore.Mvc;
using SWP.Core.Dtos;
using SWP.Core.Dtos.SeriveTicketDto;
using SWP.Core.Interfaces.Services;

namespace SWP.Api.Controllers
{
    /// <summary>
    /// Controller cho Service Ticket
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceTicketController : ControllerBase
    {
        private readonly IServiceTicketService _serviceTicketService;

        public ServiceTicketController(IServiceTicketService serviceTicketService)
        {
            _serviceTicketService = serviceTicketService;
        }

        /// <summary>
        /// Lấy danh sách Service Ticket có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Service Ticket</returns>
        [HttpPost("paging")]
        public async Task<IActionResult> GetPaging([FromBody] ServiceTicketFilterDtoRequest filter)
        {
            var result = await _serviceTicketService.GetPagingAsync(filter);
            var response = ApiResponse<object>.SuccessResponse(
                new
                {
                    items = result.Items,
                    total = result.Total,
                    page = result.Page,
                    pageSize = result.PageSize
                },
                "Lấy danh sách service ticket thành công"
            );
            return Ok(response);
        }

        #region Staff Operations

        /// <summary>
        /// Lấy Service Ticket theo ID
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <returns>Chi tiết Service Ticket</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _serviceTicketService.GetByIdAsync(id);
            return Ok(ApiResponse<ServiceTicketDetailDto>.SuccessResponse(result, "Lấy chi tiết service ticket thành công"));
        }

        /// <summary>
        /// Tạo mới Service Ticket (Staff)
        /// </summary>
        /// <param name="request">Thông tin Service Ticket mới</param>
        /// <returns>ID của Service Ticket vừa tạo</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceTicketCreateDto request)
        {
            var id = await _serviceTicketService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id }, 
                ApiResponse<object>.SuccessResponse(new { serviceTicketId = id }, "Tạo service ticket thành công"));
        }

        /// <summary>
        /// Cập nhật Service Ticket (Staff: chỉnh sửa thông tin khách hàng, xe, status)
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServiceTicketUpdateDto request)
        {
            var result = await _serviceTicketService.UpdateAsync(id, request);
            return Ok(ApiResponse<object>.SuccessResponse(new { affectedRows = result }, "Cập nhật service ticket thành công"));
        }

        /// <summary>
        /// Xóa Service Ticket
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _serviceTicketService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(new { affectedRows = result }, "Xóa service ticket thành công"));
        }

        /// <summary>
        /// Assign Service Ticket cho technical staff (tạo TechnicalTask)
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin assign</param>
        /// <returns>ID của TechnicalTask vừa tạo</returns>
        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignToTechnical(int id, [FromBody] ServiceTicketAssignDto request)
        {
            var result = await _serviceTicketService.AssignToTechnicalAsync(id, request);
            return Ok(ApiResponse<object>.SuccessResponse(new { technicalTaskId = result }, "Assign service ticket cho technical staff thành công"));
        }

        /// <summary>
        /// Thêm part vào Service Ticket (Staff có thể thêm)
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin part</param>
        /// <returns>ID của Service Ticket Detail vừa tạo</returns>
        [HttpPost("{id}/parts")]
        public async Task<IActionResult> AddPart(int id, [FromBody] ServiceTicketAddPartDto request)
        {
            var detailId = await _serviceTicketService.AddPartAsync(id, request);
            return Ok(ApiResponse<object>.SuccessResponse(new { serviceTicketDetailId = detailId }, "Thêm part vào service ticket thành công"));
        }

        /// <summary>
        /// Thêm garage service vào Service Ticket (Staff có thể thêm)
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin garage service</param>
        /// <returns>ID của Service Ticket Detail vừa tạo</returns>
        [HttpPost("{id}/garage-services")]
        public async Task<IActionResult> AddGarageService(int id, [FromBody] ServiceTicketAddGarageServiceDto request)
        {
            var detailId = await _serviceTicketService.AddGarageServiceAsync(id, request);
            return Ok(ApiResponse<object>.SuccessResponse(new { serviceTicketDetailId = detailId }, "Thêm garage service vào service ticket thành công"));
        }

        /// <summary>
        /// Xóa part/garage service khỏi Service Ticket
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="detailId">ID của Service Ticket Detail</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpDelete("{id}/details/{detailId}")]
        public async Task<IActionResult> RemoveDetail(int id, int detailId)
        {
            var result = await _serviceTicketService.RemoveDetailAsync(id, detailId);
            return Ok(ApiResponse<object>.SuccessResponse(new { affectedRows = result }, "Xóa detail khỏi service ticket thành công"));
        }

        /// <summary>
        /// Duyệt parts và services do Mechanic đề xuất (Staff)
        /// </summary>
        /// <param name="technicalTaskId">ID của TechnicalTask</param>
        /// <param name="request">Danh sách parts và services được duyệt</param>
        /// <param name="staffId">ID của staff duyệt</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpPost("technical-tasks/{technicalTaskId}/approve")]
        public async Task<IActionResult> ApproveMechanicProposal(int technicalTaskId, [FromBody] ServiceTicketUpdatePartsServicesDto request, [FromQuery] int staffId)
        {
            var result = await _serviceTicketService.ApproveMechanicProposalAsync(technicalTaskId, request, staffId);
            return Ok(ApiResponse<object>.SuccessResponse(new { affectedRows = result }, "Duyệt đề xuất của mechanic thành công"));
        }

        #endregion

        #region Mechanic Operations

        /// <summary>
        /// Lấy danh sách tasks của Mechanic có phân trang
        /// </summary>
        /// <param name="mechanicId">ID của Mechanic</param>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách tasks</returns>
        [HttpPost("mechanic/{mechanicId}/tasks")]
        public async Task<IActionResult> GetMyTasks(int mechanicId, [FromBody] ServiceTicketFilterDtoRequest filter)
        {
            var result = await _serviceTicketService.GetMyTasksAsync(mechanicId, filter);
            return Ok(ApiResponse<object>.SuccessResponse(
                new
                {
                    items = result.Items,
                    total = result.Total,
                    page = result.Page,
                    pageSize = result.PageSize
                },
                "Lấy danh sách tasks thành công"
            ));
        }

        /// <summary>
        /// Lấy chi tiết task của Mechanic
        /// </summary>
        /// <param name="technicalTaskId">ID của TechnicalTask</param>
        /// <param name="mechanicId">ID của Mechanic</param>
        /// <returns>Chi tiết task</returns>
        [HttpGet("mechanic/{mechanicId}/tasks/{technicalTaskId}")]
        public async Task<IActionResult> GetMyTaskDetail(int technicalTaskId, int mechanicId)
        {
            var result = await _serviceTicketService.GetMyTaskDetailAsync(technicalTaskId, mechanicId);
            return Ok(ApiResponse<MechanicTaskDto>.SuccessResponse(result, "Lấy chi tiết task thành công"));
        }

        /// <summary>
        /// Mechanic đề xuất parts và services (gửi cho Staff duyệt)
        /// </summary>
        /// <param name="technicalTaskId">ID của TechnicalTask</param>
        /// <param name="request">Danh sách parts và services</param>
        /// <param name="mechanicId">ID của Mechanic</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpPost("technical-tasks/{technicalTaskId}/propose")]
        public async Task<IActionResult> ProposePartsServices(int technicalTaskId, [FromBody] ServiceTicketUpdatePartsServicesDto request, [FromQuery] int mechanicId)
        {
            var result = await _serviceTicketService.ProposePartsServicesAsync(technicalTaskId, request, mechanicId);
            return Ok(ApiResponse<object>.SuccessResponse(new { affectedRows = result }, "Đề xuất parts và services thành công"));
        }

        /// <summary>
        /// Mechanic bắt đầu làm task (duyệt và bắt đầu)
        /// </summary>
        /// <param name="technicalTaskId">ID của TechnicalTask</param>
        /// <param name="mechanicId">ID của Mechanic</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpPost("technical-tasks/{technicalTaskId}/start")]
        public async Task<IActionResult> StartTask(int technicalTaskId, [FromQuery] int mechanicId)
        {
            var result = await _serviceTicketService.StartTaskAsync(technicalTaskId, mechanicId);
            return Ok(ApiResponse<object>.SuccessResponse(new { affectedRows = result }, "Bắt đầu task thành công"));
        }

        /// <summary>
        /// Mechanic confirm task đã hoàn thành
        /// </summary>
        /// <param name="technicalTaskId">ID của TechnicalTask</param>
        /// <param name="mechanicId">ID của Mechanic</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpPost("technical-tasks/{technicalTaskId}/confirm")]
        public async Task<IActionResult> ConfirmTask(int technicalTaskId, [FromQuery] int mechanicId)
        {
            var result = await _serviceTicketService.ConfirmTaskAsync(technicalTaskId, mechanicId);
            return Ok(ApiResponse<object>.SuccessResponse(new { affectedRows = result }, "Confirm task thành công"));
        }

        #endregion
    }
}

