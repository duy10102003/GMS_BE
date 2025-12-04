using Microsoft.AspNetCore.Mvc;
using SWP.Core.Dtos;
using SWP.Core.Dtos.PartDto;
using SWP.Core.Interfaces.Services;

namespace SWP.Api.Controllers
{
    /// <summary>
    /// Controller cho Part
    /// Created by: DuyLC(02/12/2025)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PartController : ControllerBase
    {
        private readonly IPartService _partService;

        public PartController(IPartService partService)
        {
            _partService = partService;
        }

        /// <summary>
        /// Lấy tất cả Part (cho select)
        /// </summary>
        /// <returns>Danh sách Part</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _partService.GetAllForSelectAsync();
            return Ok(ApiResponse<List<PartSelectDto>>.SuccessResponse(result, "Lấy danh sách part thành công"));
        }

        /// <summary>
        /// Tìm kiếm Part cho select (với search keyword)
        /// </summary>
        /// <param name="request">Request tìm kiếm</param>
        /// <returns>Danh sách Part</returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchForSelect([FromBody] PartSearchRequest request)
        {
            var result = await _partService.SearchForSelectAsync(request);
            return Ok(ApiResponse<List<PartSelectDto>>.SuccessResponse(result, "Tìm kiếm part thành công"));
        }

        /// <summary>
        /// Lấy danh sách Part có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Part</returns>
        [HttpPost("paging")]
        public async Task<IActionResult> GetPaging([FromBody] PartFilterDtoRequest filter)
        {
            var result = await _partService.GetPagingAsync(filter);
            return Ok(ApiResponse<object>.SuccessResponse(
                new
                {
                    items = result.Items,
                    total = result.Total,
                    page = result.Page,
                    pageSize = result.PageSize
                },
                "Lấy danh sách part thành công"
            ));
        }

        /// <summary>
        /// Kiểm tra mã Part đã tồn tại hay chưa
        /// </summary>
        /// <param name="partCode">Mã Part cần kiểm tra</param>
        /// <param name="excludeId">ID của Part hiện tại (null khi tạo mới, có giá trị khi update để loại trừ chính record đang sửa)</param>
        /// <returns>True nếu đã tồn tại, False nếu chưa</returns>
        [HttpGet("check-code")]
        public async Task<IActionResult> CheckCodeExists([FromQuery] string partCode, [FromQuery] int? excludeId = null)
        {
            var result = await _partService.CheckCodeExistsAsync(partCode, excludeId);
            return Ok(ApiResponse<object>.SuccessResponse(new { exists = result }, "Kiểm tra mã part thành công"));
        }

        /// <summary>
        /// Lấy Part theo ID
        /// </summary>
        /// <param name="id">ID của Part</param>
        /// <returns>Chi tiết Part</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _partService.GetByIdAsync(id);
            return Ok(ApiResponse<PartDetailDto>.SuccessResponse(result, "Lấy chi tiết part thành công"));
        }

        /// <summary>
        /// Tạo mới Part
        /// </summary>
        /// <param name="request">Thông tin Part mới</param>
        /// <returns>ID của Part vừa tạo</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PartCreateDto request)
        {
            var id = await _partService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id }, 
                ApiResponse<object>.SuccessResponse(new { partId = id }, "Tạo part thành công"));
        }

        /// <summary>
        /// Cập nhật Part
        /// </summary>
        /// <param name="id">ID của Part</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PartUpdateDto request)
        {
            var result = await _partService.UpdateAsync(id, request);
            return Ok(ApiResponse<object>.SuccessResponse(new { affectedRows = result }, "Cập nhật part thành công"));
        }

        /// <summary>
        /// Xóa Part
        /// </summary>
        /// <param name="id">ID của Part</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _partService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(new { affectedRows = result }, "Xóa part thành công"));
        }

        ///// <summary>
        ///// Kiểm tra mã Part đã tồn tại hay chưa
        ///// </summary>
        ///// <param name="partCode">Mã Part cần kiểm tra</param>
        ///// <param name="excludeId">ID của Part hiện tại (null khi tạo mới, có giá trị khi update để loại trừ chính record đang sửa)</param>
        ///// <returns>True nếu đã tồn tại, False nếu chưa</returns>
        //[HttpGet("check-code")]
        //public async Task<IActionResult> CheckCodeExists([FromQuery] string partCode, [FromQuery] int? excludeId = null)
        //{
        //    var result = await _partService.CheckCodeExistsAsync(partCode, excludeId);
        //    return Ok(ApiResponse<object>.SuccessResponse(new { exists = result }, "Kiểm tra mã part thành công"));
        //}
    }
}

