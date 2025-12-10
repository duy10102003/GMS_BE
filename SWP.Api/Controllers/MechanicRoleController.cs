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
    }
}
