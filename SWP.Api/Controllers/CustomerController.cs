using Microsoft.AspNetCore.Mvc;
using SWP.Core.Dtos;
using SWP.Core.Dtos.CustomerDto;
using SWP.Core.Interfaces.Services;

namespace SWP.Api.Controllers
{
    /// <summary>
    /// Controller cho Customer
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Tìm kiếm Customer cho select (với search keyword)
        /// </summary>
        /// <param name="request">Request tìm kiếm</param>
        /// <returns>Danh sách Customer</returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchForSelect([FromBody] CustomerSearchRequest request)
        {
            var result = await _customerService.SearchForSelectAsync(request);
            return Ok(ApiResponse<List<CustomerSelectDto>>.SuccessResponse(result, "Tìm kiếm customer thành công"));
        }
    }
}


