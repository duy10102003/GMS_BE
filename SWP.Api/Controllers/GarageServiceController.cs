using Microsoft.AspNetCore.Mvc;
using SWP.Core.Dtos;
using SWP.Core.Dtos.GarageServiceDto;
using SWP.Core.Interfaces.Services;

namespace SWP.Api.Controllers
{
    /// <summary>
    /// Controller cho Garage Service (Manager)
    /// Created by: DuyLC(02/12/2025)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GarageServiceController : ControllerBase
    {
        private readonly IGarageServiceService _garageServiceService;

        public GarageServiceController(IGarageServiceService garageServiceService)
        {
            _garageServiceService = garageServiceService;
        }

        /// <summary>
        /// Lấy danh sách Garage Service có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Garage Service</returns>
        [HttpPost("paging")]
        public async Task<IActionResult> GetPaging([FromBody] GarageServiceFilterDtoRequest filter)
        {
            var result = await _garageServiceService.GetPagingAsync(filter);
            return Ok(ApiResponse<object>.SuccessResponse(
                new
                {
                    items = result.Items,
                    total = result.Total,
                    page = result.Page,
                    pageSize = result.PageSize
                },
                "Lấy danh sách garage service thành công"
            ));
        }

        /// <summary>
        /// Lấy Garage Service theo ID
        /// </summary>
        /// <param name="id">ID của Garage Service</param>
        /// <returns>Chi tiết Garage Service</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _garageServiceService.GetByIdAsync(id);
            return Ok(ApiResponse<GarageServiceDetailDto>.SuccessResponse(result, "Lấy chi tiết garage service thành công"));
        }

        /// <summary>
        /// Tạo mới Garage Service
        /// </summary>
        /// <param name="request">Thông tin Garage Service mới</param>
        /// <returns>ID của Garage Service vừa tạo</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GarageServiceCreateDto request)
        {
            var id = await _garageServiceService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id }, 
                ApiResponse<object>.SuccessResponse(new { garageServiceId = id }, "Tạo garage service thành công"));
        }

        /// <summary>
        /// Cập nhật Garage Service
        /// </summary>
        /// <param name="id">ID của Garage Service</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] GarageServiceUpdateDto request)
        {
            var result = await _garageServiceService.UpdateAsync(id, request);
            return Ok(ApiResponse<object>.SuccessResponse(new { affectedRows = result }, "Cập nhật garage service thành công"));
        }

        /// <summary>
        /// Xóa Garage Service
        /// </summary>
        /// <param name="id">ID của Garage Service</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _garageServiceService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(new { affectedRows = result }, "Xóa garage service thành công"));
        }

        /// <summary>
        /// Tìm kiếm Garage Service cho select (với search keyword)
        /// </summary>
        /// <param name="request">Request tìm kiếm</param>
        /// <returns>Danh sách Garage Service</returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchForSelect([FromBody] GarageServiceSearchRequest request)
        {
            var result = await _garageServiceService.SearchForSelectAsync(request);
            return Ok(ApiResponse<List<GarageServiceSelectDto>>.SuccessResponse(result, "Tìm kiếm garage service thành công"));
        }
    }
}


