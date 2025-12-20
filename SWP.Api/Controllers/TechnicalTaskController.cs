using Microsoft.AspNetCore.Mvc;
using SWP.Core.Dtos;
using SWP.Core.Dtos.TechnicalTaskDto;
using SWP.Core.Interfaces.Services;

namespace SWP.Api.Controllers
{
    /// <summary>
    /// Controller cho Technical Task
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TechnicalTaskController : ControllerBase
    {
        private readonly ITechnicalTaskService _technicalTaskService;

        public TechnicalTaskController(ITechnicalTaskService technicalTaskService)
        {
            _technicalTaskService = technicalTaskService;
        }

        /// <summary>
        /// Lấy danh sách Technical Task có phân trang (cho technical staff)
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Technical Task</returns>
        [HttpPost("paging")]
        public async Task<IActionResult> GetPaging([FromBody] TechnicalTaskFilterDtoRequest filter)
        {
            var result = await _technicalTaskService.GetPagingAsync(filter);
            return Ok(ApiResponse<object>.SuccessResponse(
                new
                {
                    items = result.Items,
                    total = result.Total,
                    page = result.Page,
                    pageSize = result.PageSize
                },
                "Lấy danh sách technical task thành công"
            ));
        }

        /// <summary>
        /// Lấy Technical Task theo ID
        /// </summary>
        /// <param name="id">ID của Technical Task</param>
        /// <returns>Chi tiết Technical Task</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _technicalTaskService.GetByIdAsync(id);
            return Ok(ApiResponse<TechnicalTaskDetailDto>.SuccessResponse(result, "Lấy chi tiết technical task thành công"));
        }

        /// <summary>
        /// Technical staff điều chỉnh Parts và Services trong Service Ticket
        /// </summary>
        /// <param name="id">ID của Technical Task</param>
        /// <param name="request">Danh sách Parts và Services mới</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpPut("{id}/adjust")]
        public async Task<IActionResult> AdjustPartsServices(int id, [FromBody] TechnicalTaskAdjustPartsServicesDto request)
        {
            var result = await _technicalTaskService.AdjustPartsServicesAsync(id, request);
            return Ok(ApiResponse<object>.SuccessResponse(new { affectedRows = result }, "Điều chỉnh parts và services thành công"));
        }

        /// <summary>
        /// Staff xác nhận điều chỉnh từ technical (chuyển status sang InProgress)
        /// </summary>
        /// <param name="id">ID của Technical Task</param>
        /// <param name="confirmedBy">ID của staff xác nhận</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmAdjustment(int id, [FromQuery] int confirmedBy)
        {
            var result = await _technicalTaskService.ConfirmAdjustmentAsync(id, confirmedBy);
            return Ok(ApiResponse<object>.SuccessResponse(new { affectedRows = result }, "Xác nhận điều chỉnh thành công"));
        }

        /// <summary>
        /// Technical staff hoàn thành công việc (chuyển status sang Completed)
        /// </summary>
        /// <param name="id">ID của Technical Task</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var result = await _technicalTaskService.CompleteTaskAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(new { affectedRows = result }, "Hoàn thành công việc thành công"));
        }
    }
}


