using Microsoft.AspNetCore.Mvc;
using SWP.Core.Dtos;
using SWP.Core.Dtos.BookingDto;
using SWP.Core.Interfaces.Services;

namespace SWP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("paging")]
        public async Task<IActionResult> GetPaging([FromBody] BookingFilterDtoRequest filter)
        {
            var result = await _bookingService.GetPagingAsync(filter);
            var response = ApiResponse<object>.SuccessResponse(
                new
                {
                    items = result.Items,
                    total = result.Total,
                    page = result.Page,
                    pageSize = result.PageSize
                },
                "Lấy danh sách booking thành công"
            );
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _bookingService.GetByIdAsync(id);
            var response = ApiResponse<BookingDetailDto>.SuccessResponse(
                result,
                "Lấy chi tiết booking thành công"
            );
            return Ok(response);
        }

        [HttpPost("guest")]
        public async Task<IActionResult> CreateForGuest([FromBody] BookingCreateGuestDto request)
        {
            var id = await _bookingService.CreateForGuestAsync(request);
            var response = ApiResponse<object>.SuccessResponse(
                new { bookingId = id },
                "Tạo booking cho guest thành công"
            );
            return CreatedAtAction(nameof(GetById), new { id }, response);
        }

        [HttpPost("user")]
        public async Task<IActionResult> CreateForUser([FromBody] BookingCreateForUserDto request)
        {
            var id = await _bookingService.CreateForUserAsync(request);
            var response = ApiResponse<object>.SuccessResponse(
                new { bookingId = id },
                "Tạo booking cho user thành công"
            );
            return CreatedAtAction(nameof(GetById), new { id }, response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookingCreateDto request)
        {
            var result = await _bookingService.UpdateAsync(id, request);
            var response = ApiResponse<object>.SuccessResponse(
                new { affectedRows = result },
                "Cập nhật booking thành công"
            );
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bookingService.DeleteAsync(id);
            var response = ApiResponse<object>.SuccessResponse(
                new { affectedRows = result },
                "Xóa booking thành công"
            );
            return Ok(response);
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] BookingChangeStatusDto request)
        {
            var result = await _bookingService.ChangeStatusAsync(id, request);
            var response = ApiResponse<object>.SuccessResponse(
                new { affectedRows = result },
                "Cap nhat trang thai booking thanh cong"
            );
            return Ok(response);
        }
    }
}
