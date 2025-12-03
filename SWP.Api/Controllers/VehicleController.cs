using Microsoft.AspNetCore.Mvc;
using SWP.Core.Dtos;
using SWP.Core.Dtos.VehicleDto;
using SWP.Core.Interfaces.Services;

namespace SWP.Api.Controllers
{
    /// <summary>
    /// Controller cho Vehicle
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        /// <summary>
        /// Tìm kiếm Vehicle cho select (với search keyword và filter theo customer)
        /// </summary>
        /// <param name="request">Request tìm kiếm</param>
        /// <returns>Danh sách Vehicle</returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchForSelect([FromBody] VehicleSearchRequest request)
        {
            var result = await _vehicleService.SearchForSelectAsync(request);
            return Ok(ApiResponse<List<VehicleSelectDto>>.SuccessResponse(result, "Tìm kiếm vehicle thành công"));
        }
    }
}

