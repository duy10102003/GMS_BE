using Microsoft.AspNetCore.Mvc;
using SWP.Core.Dtos;
using SWP.Core.Dtos.MechanicReportDto;
using SWP.Core.Interfaces.Services;

namespace SWP.Api.Controllers
{
    /// <summary>
    /// API báo cáo cho mechanic
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MechanicReportController : ControllerBase
    {
        private readonly IMechanicReportService _mechanicReportService;

        public MechanicReportController(IMechanicReportService mechanicReportService)
        {
            _mechanicReportService = mechanicReportService;
        }

        /// <summary>
        /// Mechanic: xem báo cáo tổng hợp của chính mình
        /// </summary>
        /// <param name="mechanicId">ID user của mechanic đang đăng nhập</param>
        [HttpGet("{mechanicId:int}/summary")]
        public async Task<IActionResult> GetMySummary(int mechanicId)
        {
            var result = await _mechanicReportService.GetMySummaryAsync(mechanicId);
            return Ok(ApiResponse<MechanicReportSummaryDto>.SuccessResponse(
                result,
                "Lấy báo cáo mechanic thành công"
            ));
        }
    }
}

