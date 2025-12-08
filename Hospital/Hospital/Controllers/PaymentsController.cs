using Hospital.Application.DTO.PaymentDTOs;
using Hospital.Application.Interfaces.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly PaymobOptions _paymobOptions;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, IOptions<PaymobOptions> paymobOptions, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _paymobOptions = paymobOptions.Value;
            _logger = logger;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request,CancellationToken ct)
        {
            _logger.LogInformation("create payment by credite card called at {time}", DateTime.Now);

            try
            {
                // Use "uid" instead of ClaimTypes.NameIdentifier
                var userId = User.FindFirst("uid")?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var paymentToken = await _paymentService.CreatePaymobPaymentForAppointmentAsync(
                    request.AppointmentId,
                    userId,
                    ct);

                return Ok(new
                {
                    success = true,
                    paymentToken,
                    iframeUrl = $"https://accept.paymob.com/api/acceptance/iframes/{_paymobOptions.IframeId}?payment_token={paymentToken}"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Payment creation failed", error = ex.Message });
            }
        }


        [HttpPost("callback")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymobCallback([FromBody] JsonElement rawCallback, CancellationToken ct)
        {
            _logger.LogInformation("Call back paymob called at {time}", DateTime.Now);


            try
            {
                var callbackJson = rawCallback.GetRawText();
                Console.WriteLine($"Paymob Callback Received: {callbackJson}");

                var obj = rawCallback.GetProperty("obj");

                var orderId = obj.GetProperty("order").GetProperty("merchant_order_id").GetString();
                var transactionId = obj.GetProperty("id").ToString();
                var success = obj.GetProperty("success").GetBoolean();
                var amountCents = obj.GetProperty("amount_cents").GetInt32();
                var currency = obj.GetProperty("currency").GetString();

                var hmac = rawCallback.GetProperty("hmac").GetString();
                if (!VerifyHmacSignature(obj, hmac))
                {
                    return Unauthorized(new { success = false, message = "Invalid HMAC signature" });
                }

                var dto = new PaymobCallbackDto
                {
                    PaymentId = transactionId,
                    OrderId = orderId,
                    Status = success ? "success" : "failed",
                    Amount = amountCents / 100m,
                    Currency = currency,
                    TransactionTime = DateTime.UtcNow.ToString("o")
                };

                var payment = await _paymentService.HandlePaymobCallbackAsync(dto, ct);

                if (payment == null)
                    return NotFound(new { success = false, message = "Payment not found" });

                return Ok(new { success = true, message = "Callback processed successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Callback Error: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Callback processing failed", error = ex.Message });
            }
        }

        private bool VerifyHmacSignature(JsonElement obj, string receivedHmac)
        {
            try
            {
                _logger.LogInformation("Verify Hmac Signature called at {time}", DateTime.Now);

                var amountCents = obj.GetProperty("amount_cents").GetInt32().ToString();
                var createdAt = obj.GetProperty("created_at").GetString();
                var currency = obj.GetProperty("currency").GetString();
                var errorOccurred = obj.GetProperty("error_occured").GetBoolean().ToString().ToLower();
                var hasParentTransaction = obj.GetProperty("has_parent_transaction").GetBoolean().ToString().ToLower();
                var id = obj.GetProperty("id").ToString();
                var integrationId = obj.GetProperty("integration_id").GetInt32().ToString();
                var isAuth = obj.GetProperty("is_auth").GetBoolean().ToString().ToLower();
                var is3dSecure = obj.GetProperty("is_3d_secure").GetBoolean().ToString().ToLower();
                var isCapture = obj.GetProperty("is_capture").GetBoolean().ToString().ToLower();
                var isRefunded = obj.GetProperty("is_refunded").GetBoolean().ToString().ToLower();
                var isStandalonePayment = obj.GetProperty("is_standalone_payment").GetBoolean().ToString().ToLower();
                var isVoided = obj.GetProperty("is_voided").GetBoolean().ToString().ToLower();
                var orderId = obj.GetProperty("order").GetProperty("id").GetInt32().ToString();
                var owner = obj.GetProperty("owner").GetInt32().ToString();
                var pending = obj.GetProperty("pending").GetBoolean().ToString().ToLower();
                var sourceDataPan = obj.GetProperty("source_data").GetProperty("pan").GetString();
                var sourceDataSubType = obj.GetProperty("source_data").GetProperty("sub_type").GetString();
                var sourceDataType = obj.GetProperty("source_data").GetProperty("type").GetString();
                var success = obj.GetProperty("success").GetBoolean().ToString().ToLower();

                var concatenated = string.Concat(
                    amountCents,
                    createdAt,
                    currency,
                    errorOccurred,
                    hasParentTransaction,
                    id,
                    integrationId,
                    isAuth,
                    isCapture,
                    isRefunded,
                    isStandalonePayment,
                    is3dSecure,
                    isVoided,
                    orderId,
                    owner,
                    pending,
                    sourceDataPan,
                    sourceDataSubType,
                    sourceDataType,
                    success
                );

                using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_paymobOptions.HmacSecret));
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenated));
                var calculatedHmac = BitConverter.ToString(hash).Replace("-", "").ToLower();

                return calculatedHmac == receivedHmac?.ToLower();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HMAC Verification Error: {ex.Message}");
                return false;
            }
        }
    }

    public class CreatePaymentRequest
    {
        public int AppointmentId { get; set; }
    }
}
