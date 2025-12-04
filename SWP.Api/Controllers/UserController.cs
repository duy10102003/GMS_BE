using Microsoft.AspNetCore.Mvc;
using SWP.Core.Dtos;
using SWP.Core.Dtos.UserDto;
using SWP.Core.Interfaces.Services;

namespace SWP.Api.Controllers
{
    /// <summary>
    /// Controller cho User
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lấy danh sách Technical Staff với trạng thái rảnh/không rảnh
        /// </summary>
        /// <param name="roleName">Tên role (ví dụ: "TechnicalStaff", "Mechanic")</param>
        /// <returns>Danh sách Technical Staff</returns>
        [HttpGet("technical-staff")]
        public async Task<IActionResult> GetTechnicalStaff([FromQuery] string roleName = "TechnicalStaff")
        {
            var result = await _userService.GetTechnicalStaffWithAvailabilityAsync(roleName);
            return Ok(ApiResponse<List<TechnicalStaffSelectDto>>.SuccessResponse(result, "Lấy danh sách technical staff thành công"));
        }
    }
}

