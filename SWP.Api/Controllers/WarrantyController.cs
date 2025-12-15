using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP.Core.Dtos.InvoiceDto;
using SWP.Core.Dtos;
using SWP.Core.Interfaces.Services;
using SWP.Core.Services;
using SWP.Core.Dtos.WarrantyDto;

namespace SWP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantyController : ControllerBase
    {
        private readonly IWarrantyService _warrantyService;

        public WarrantyController(IWarrantyService warrantyService)
        {
            _warrantyService = warrantyService;
        }


        /// <summary>
        /// Lấy danh sách bảo hành có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách bảo hành</returns>
        [HttpPost("paging")]
        public async Task<IActionResult> GetPaging([FromBody] WarrantyFilterDtoRequest filter)
        {
            var result = await _warrantyService.GetPagingAsync(filter);
            return Ok(ApiResponse<object>.SuccessResponse(
                new
                {
                    items = result.Items,
                    total = result.Total,
                    page = result.Page,
                    pageSize = result.PageSize
                },
                "Lấy danh sách bảo hành thành công"
            ));
        }

        /// <summary>
        /// Lấy danh sách bảo hành có phân trang của customer
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách bảo hành</returns>
        [HttpPost("{id}/customer/paging")]
        public async Task<IActionResult> GetPagingByCustomer(int id, [FromBody] WarrantyFilterDtoRequest filter)
        {
            var result = await _warrantyService.GetByCustomerPagingAsync(id, filter);
            return Ok(ApiResponse<object>.SuccessResponse(
                new
                {
                    items = result.Items,
                    total = result.Total,
                    page = result.Page,
                    pageSize = result.PageSize
                },
                "Lấy danh sách bảo hành thành công"
            ));
        }

    }
}
