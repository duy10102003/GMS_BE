using Microsoft.AspNetCore.Mvc;
using SWP.Core.Dtos;
using SWP.Core.Dtos.InvoiceDto;
using SWP.Core.Interfaces.Repositories;
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
       // private readonly IInvoiceRepo _invoiceRepo;
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
        /// Lấy danh sách Invoice theo UserId (thông qua Customer.UserId)
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <param name="page">Trang hiện tại</param>
        /// <param name="pageSize">Số bản ghi mỗi trang</param>
        /// <returns>Danh sách Invoice</returns>
        //[HttpGet("by-user/{userId:int}")]
        //public async Task<IActionResult> GetByUserId(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        //{
        //    var filter = new InvoiceFilterDtoRequest
        //    {
                
        //        Page = page,
        //        PageSize = pageSize
        //    };

        //    var result = await _invoiceService.GetPagingAsync(filter);

        //    return Ok(ApiResponse<object>.SuccessResponse(
        //        new
        //        {
        //            items = result.Items,
        //            total = result.Total,
        //            page = result.Page,
        //            pageSize = result.PageSize
        //        },
        //        "Lấy danh sách invoice theo user thành công"
        //    ));
        //}

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

        /// <summary>
        /// Chuyển trạng thái Invoice sang đã thanh toán (status = 1)
        /// </summary>
        /// <param name="id">ID của Invoice</param>
        /// <param name="modifiedBy">ID của người thực hiện</param>
        /// <returns>Kết quả</returns>
        [HttpPut("{id}/status/paid")]
        public async Task<IActionResult> ChangeStatusToPaid(int id, [FromQuery] int modifiedBy)
        {
            await _invoiceService.ChangeStatusToPaidAsync(id, modifiedBy);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Chuyển trạng thái invoice sang đã thanh toán thành công"));
        }
        [HttpPost("{id}/customer/paging")]
        public async Task<IActionResult> GetInvoicesForCustomer(
            int id,
            [FromBody] InvoiceFilterDtoRequest filter)
        {
            filter ??= new InvoiceFilterDtoRequest();

            var result = await _invoiceService.GetPagingInvoiceForCustomerAsync(id, filter);

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

    }
}


