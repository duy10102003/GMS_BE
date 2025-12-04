using Microsoft.AspNetCore.Mvc;
using SWP.Core.Dtos;
using SWP.Core.Dtos.PartCategoryDto;
using SWP.Core.Interfaces.Services;

namespace SWP.Api.Controllers
{
    /// <summary>
    /// Controller cho Part Category
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PartCategoryController : ControllerBase
    {
        private readonly IPartCategoryService _partCategoryService;

        public PartCategoryController(IPartCategoryService partCategoryService)
        {
            _partCategoryService = partCategoryService;
        }

        /// <summary>
        /// Lấy danh sách Part Category có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Part Category</returns>
        [HttpPost("paging")]
        public async Task<IActionResult> GetPaging([FromBody] PartCategoryFilterDtoRequest filter)
        {
            var result = await _partCategoryService.GetPagingAsync(filter);
            return Ok(ApiResponse<object>.SuccessResponse(
                new
                {
                    items = result.Items,
                    total = result.Total,
                    page = result.Page,
                    pageSize = result.PageSize
                },
                "Lấy danh sách part category thành công"
            ));
        }

        /// <summary>
        /// Lấy tất cả Part Category (cho select)
        /// </summary>
        /// <returns>Danh sách Part Category</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _partCategoryService.GetAllForSelectAsync();
            return Ok(ApiResponse<List<PartCategorySelectDto>>.SuccessResponse(result, "Lấy danh sách part category thành công"));
        }

        /// <summary>
        /// Lấy Part Category theo ID
        /// </summary>
        /// <param name="id">ID của Part Category</param>
        /// <returns>Chi tiết Part Category</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _partCategoryService.GetByIdAsync(id);
            return Ok(ApiResponse<PartCategoryDetailDto>.SuccessResponse(result, "Lấy chi tiết part category thành công"));
        }

        /// <summary>
        /// Tạo mới Part Category
        /// </summary>
        /// <param name="request">Thông tin Part Category mới</param>
        /// <returns>ID của Part Category vừa tạo</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PartCategoryCreateDto request)
        {
            var id = await _partCategoryService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id }, 
                ApiResponse<object>.SuccessResponse(new { partCategoryId = id }, "Tạo part category thành công"));
        }

        /// <summary>
        /// Cập nhật Part Category
        /// </summary>
        /// <param name="id">ID của Part Category</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PartCategoryUpdateDto request)
        {
            var result = await _partCategoryService.UpdateAsync(id, request);
            return Ok(ApiResponse<object>.SuccessResponse(new { affectedRows = result }, "Cập nhật part category thành công"));
        }

        /// <summary>
        /// Xóa Part Category
        /// </summary>
        /// <param name="id">ID của Part Category</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _partCategoryService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(new { affectedRows = result }, "Xóa part category thành công"));
        }

        /// <summary>
        /// Kiểm tra mã Part Category đã tồn tại hay chưa
        /// </summary>
        /// <param name="partCategoryCode">Mã Part Category cần kiểm tra</param>
        /// <param name="excludeId">ID của Part Category hiện tại (null khi tạo mới, có giá trị khi update để loại trừ chính record đang sửa)</param>
        /// <returns>True nếu đã tồn tại, False nếu chưa</returns>
        [HttpGet("check-code")]
        public async Task<IActionResult> CheckCodeExists([FromQuery] string partCategoryCode, [FromQuery] int? excludeId = null)
        {
            var result = await _partCategoryService.CheckCodeExistsAsync(partCategoryCode, excludeId);
            return Ok(ApiResponse<object>.SuccessResponse(new { exists = result }, "Kiểm tra mã part category thành công"));
        }
    }
}


