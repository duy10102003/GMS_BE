using Microsoft.AspNetCore.Mvc;
using SWP.Core.Dtos;
using SWP.Core.Dtos.InvoiceDto;
using SWP.Core.Interfaces.Services;

namespace SWP.Api.Controllers
{
    /// <summary>
    /// Controller cho Invoice
    /// Created by: DuyLC(03/12/2025)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// Lấy danh sách Invoice có phân trang
        /// </summary>
        /// <param name="filter">Filter và phân trang</param>
        /// <returns>Danh sách Invoice</returns>
        [HttpPost("paging")]
        public async Task<IActionResult> GetPaging([FromBody] InvoiceFilterDtoRequest filter)
        {
            var result = await _invoiceService.GetPagingAsync(filter);
            return Ok(ApiResponse<object>.SuccessResponse(
                new
                {
                    items = result.Items,
                    total = result.Total,
                    page = result.Page,
                    pageSize = result.PageSize
                },
                "Lấy danh sách invoice thành công"
            ));
        }

        /// <summary>
        /// Lấy Invoice theo ID
        /// </summary>
        /// <param name="id">ID của Invoice</param>
        /// <returns>Chi tiết Invoice</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _invoiceService.GetByIdAsync(id);
            return Ok(ApiResponse<InvoiceDetailDto>.SuccessResponse(result, "Lấy chi tiết invoice thành công"));
        }

        /// <summary>
        /// Tạo Invoice từ Service Ticket đã hoàn thành
        /// </summary>
        /// <param name="request">Thông tin Invoice</param>
        /// <returns>ID của Invoice vừa tạo</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceCreateDto request)
        {
            var id = await _invoiceService.CreateFromServiceTicketAsync(request);
            return CreatedAtAction(nameof(GetById), new { id }, 
                ApiResponse<object>.SuccessResponse(new { invoiceId = id }, "Tạo invoice thành công"));
        }
    }
}

