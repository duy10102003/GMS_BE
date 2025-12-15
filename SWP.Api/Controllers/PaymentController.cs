using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SWP.Core.Dtos.PaymentDto;
using SWP.Core.Helpers.Vnpay;
using SWP.Core.Interfaces.Repositories;

namespace SWP.Api.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IInvoiceRepo _invoiceRepo;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IConfiguration config, IInvoiceRepo invoiceRepo, ILogger<PaymentController> logger)
        {
            _config = config;
            _invoiceRepo = invoiceRepo;
            _logger = logger;
        }

        [HttpPost("vnpay/create")]
        public async Task<IActionResult> CreateVnPayPayment([FromBody] VnPayCreatePaymentRequest request)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(request.InvoiceId);
            if (invoice == null)
                return NotFound("Invoice not found");

            var vnp = new VnPayLibrary();

            vnp.AddRequestData("vnp_Version", "2.1.0");
            vnp.AddRequestData("vnp_Command", "pay");
            vnp.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"] ?? string.Empty);
            vnp.AddRequestData("vnp_SecureHashType", "SHA512");
            vnp.AddRequestData("vnp_Amount", ((long)(request.Amount * 100)).ToString());
            vnp.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnp.AddRequestData("vnp_CurrCode", "VND");
            vnp.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
            vnp.AddRequestData("vnp_Locale", "vn");
            vnp.AddRequestData("vnp_OrderInfo", $"Thanh toan hoa don {invoice.InvoiceCode}");
            vnp.AddRequestData("vnp_OrderType", "other");
            vnp.AddRequestData("vnp_ReturnUrl", _config["VnPay:ReturnUrl"] ?? string.Empty);
            vnp.AddRequestData("vnp_TxnRef", invoice.InvoiceId.ToString());

            var signData = vnp.BuildRequestSignData();
            var paymentUrl = vnp.CreateRequestUrl(
                _config["VnPay:BaseUrl"] ?? string.Empty,
                _config["VnPay:HashSecret"] ?? string.Empty
            );

            _logger.LogInformation("VNPAY create request. TxnRef={TxnRef}, SignData={SignData}, PaymentUrl={PaymentUrl}",
                invoice.InvoiceId, signData, paymentUrl);

            return Ok(new
            {
                paymentUrl
            });
        }

        [HttpGet("vnpay/return")]
        public async Task<IActionResult> VnPayReturn()
        {
            var vnp = new VnPayLibrary();

            foreach (var key in Request.Query.Keys)
            {
                if (key.StartsWith("vnp_"))
                    vnp.AddResponseData(key, Request.Query[key].ToString());
            }

            var secureHash = Request.Query["vnp_SecureHash"].ToString();
            var responseSignData = vnp.BuildResponseSignData();
            var isValid = vnp.ValidateSignature(
                secureHash ?? string.Empty,
                _config["VnPay:HashSecret"] ?? string.Empty
            );

            _logger.LogInformation("VNPAY return. RawSecureHash={SecureHash}, Recomputed={Recomputed}, SignData={SignData}",
                secureHash,
                _config["VnPay:HashSecret"] != null ? vnp.BuildResponseSignData() : string.Empty,
                responseSignData);

            if (!isValid)
                return BadRequest("Invalid signature");

            var invoiceId = int.Parse(vnp.GetResponseData("vnp_TxnRef"));
            var responseCode = vnp.GetResponseData("vnp_ResponseCode");

            if (responseCode == "00")
            {
                await _invoiceRepo.MarkAsPaidAsync(invoiceId);
                return Redirect("http://localhost:5173/payment-success");
            }
            else
            {
                await _invoiceRepo.MarkAsFailedAsync(invoiceId);
                return Redirect("http://localhost:5173/payment-failed");
            }
        }
    }
}
