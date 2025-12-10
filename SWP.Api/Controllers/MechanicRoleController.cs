using Microsoft.AspNetCore.Mvc;
using SWP.Core.Dtos;
using SWP.Core.Dtos.MechanicRoleDto;
using SWP.Core.Interfaces.Services;

namespace SWP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MechanicRoleController : ControllerBase
    {
        private readonly IMechanicRoleService _mechanicRoleService;

        public MechanicRoleController(IMechanicRoleService mechanicRoleService)
        {
            _mechanicRoleService = mechanicRoleService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] MechanicRoleCreateDto request)
        {
            var id = await _mechanicRoleService.CreateRoleAsync(request);
            return Ok(ApiResponse<object>.SuccessResponse(new { mechanicRoleId = id }, "Tạo mechanic role thành công"));
        }

        [HttpPut("{mechanicRoleId:int}")]
        public async Task<IActionResult> UpdateRole(int mechanicRoleId, [FromBody] MechanicRoleUpdateDto request)
        {
            var affected = await _mechanicRoleService.UpdateRoleAsync(mechanicRoleId, request);
            return Ok(ApiResponse<object>.SuccessResponse(new { affected }, "Cập nhật mechanic role thành công"));
        }

        [HttpDelete("{mechanicRoleId:int}")]
        public async Task<IActionResult> DeleteRole(int mechanicRoleId)
        {
            var affected = await _mechanicRoleService.SoftDeleteRoleAsync(mechanicRoleId);
            return Ok(ApiResponse<object>.SuccessResponse(new { affected }, "Xóa mechanic role thành công"));
        }

        [HttpGet("{mechanicRoleId:int}")]
        public async Task<IActionResult> GetRoleById(int mechanicRoleId)
        {
            var result = await _mechanicRoleService.GetRoleByIdAsync(mechanicRoleId);
            return Ok(ApiResponse<MechanicRoleDto?>.SuccessResponse(result, "Lấy mechanic role thành công"));
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _mechanicRoleService.GetAllRolesAsync();
            return Ok(ApiResponse<List<MechanicRoleDto>>.SuccessResponse(result, "Lấy danh sách mechanic role thành công"));
        }

        [HttpGet("assignments/{userId:int}")]
        public async Task<IActionResult> GetAssignmentsByUser(int userId)
        {
            var result = await _mechanicRoleService.GetAssignmentsByUserAsync(userId);
            return Ok(ApiResponse<List<MechanicRoleAssignmentDto>>.SuccessResponse(result, "Lấy danh sách role của mechanic thành công"));
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole([FromBody] AssignMechanicRoleRequest request)
        {
            var affected = await _mechanicRoleService.AssignRoleAsync(request);
            return Ok(ApiResponse<int>.SuccessResponse(affected, "Gán role cho mechanic thành công"));
        }

        [HttpDelete("assign")]
        public async Task<IActionResult> RemoveAssignment([FromQuery] int userId, [FromQuery] int mechanicRoleId)
        {
            var affected = await _mechanicRoleService.RemoveAssignmentAsync(userId, mechanicRoleId);
            return Ok(ApiResponse<int>.SuccessResponse(affected, "Hủy gán role mechanic thành công"));
        }
        //abcd
    }
}
