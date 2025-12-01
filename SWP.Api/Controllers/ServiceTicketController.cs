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

        /// <summary>
        /// Lấy Service Ticket theo ID
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <returns>Chi tiết Service Ticket</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _serviceTicketService.GetByIdAsync(id);
            var response = ApiResponse<ServiceTicketDetailDto>.SuccessResponse(
                result,
                "Lấy chi tiết service ticket thành công"
            );
            return Ok(response);
        }

        /// <summary>
        /// Tạo mới Service Ticket
        /// </summary>
        /// <param name="request">Thông tin Service Ticket mới</param>
        /// <returns>ID của Service Ticket vừa tạo</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceTicketCreateDto request)
        {
            var id = await _serviceTicketService.CreateAsync(request);
            var response = ApiResponse<object>.SuccessResponse(
                new { serviceTicketId = id },
                "Tạo service ticket thành công"
            );
            return CreatedAtAction(nameof(GetById), new { id }, response);
        }

        /// <summary>
        /// Cập nhật Service Ticket
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ServiceTicketUpdateDto request)
        {
            var result = await _serviceTicketService.UpdateAsync(id, request);
            var response = ApiResponse<object>.SuccessResponse(
                new { affectedRows = result },
                "Cập nhật service ticket thành công"
            );
            return Ok(response);
        }

        /// <summary>
        /// Xóa Service Ticket
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _serviceTicketService.DeleteAsync(id);
            var response = ApiResponse<object>.SuccessResponse(
                new { affectedRows = result },
                "Xóa service ticket thành công"
            );
            return Ok(response);
        }

        /// <summary>
        /// Assign Service Ticket cho technical staff
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin assign</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignToTechnical(Guid id, [FromBody] ServiceTicketAssignDto request)
        {
            var result = await _serviceTicketService.AssignToTechnicalAsync(id, request);
            var response = ApiResponse<object>.SuccessResponse(
                new { affectedRows = result },
                "Assign service ticket cho technical staff thành công"
            );
            return Ok(response);
        }

        /// <summary>
        /// Technical staff nhận task
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="technicalStaffId">ID của technical staff</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptTask(Guid id, [FromQuery] Guid technicalStaffId)
        {
            var result = await _serviceTicketService.AcceptTaskAsync(id, technicalStaffId);
            var response = ApiResponse<object>.SuccessResponse(
                new { affectedRows = result },
                "Nhận task thành công"
            );
            return Ok(response);
        }

        /// <summary>
        /// Thêm part vào Service Ticket
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="request">Thông tin part</param>
        /// <returns>ID của Service Ticket Detail vừa tạo</returns>
        [HttpPost("{id}/parts")]
        public async Task<IActionResult> AddPart(Guid id, [FromBody] ServiceTicketAddPartDto request)
        {
            var detailId = await _serviceTicketService.AddPartAsync(id, request);
            var response = ApiResponse<object>.SuccessResponse(
                new { serviceTicketDetailId = detailId },
                "Thêm part vào service ticket thành công"
            );
            return Ok(response);
        }

        /// <summary>
        /// Xóa part khỏi Service Ticket
        /// </summary>
        /// <param name="id">ID của Service Ticket</param>
        /// <param name="detailId">ID của Service Ticket Detail</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpDelete("{id}/parts/{detailId}")]
        public async Task<IActionResult> RemovePart(Guid id, Guid detailId)
        {
            var result = await _serviceTicketService.RemovePartAsync(id, detailId);
            var response = ApiResponse<object>.SuccessResponse(
                new { affectedRows = result },
                "Xóa part khỏi service ticket thành công"
            );
            return Ok(response);
        }
    }
}

