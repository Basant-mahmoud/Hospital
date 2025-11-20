using Hospital.Application.DTO.PaymentDTOs;
using Hospital.Application.Interfaces.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hospital.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Patient")]
    public class PaymentsController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("{appointmentId}/paymob")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CreatePaymobPayment(int appointmentId, CancellationToken ct)
        {
            var userId = User.FindFirstValue("uid") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Error = "User not authenticated." });

            try
            {
                var paymentToken = await _paymentService.CreatePaymobPaymentForAppointmentAsync(
                    appointmentId,
                    userId,
                    ct);

                return Ok(new { PaymentToken = paymentToken });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
            }
        }

        /// <summary>
        /// Paymob server-to-server webhook to update payment status
        /// </summary>
        [HttpPost("paymob/callback")]
        [AllowAnonymous] // Paymob will call this, so no authentication
        public async Task<IActionResult> PaymobCallback([FromBody] PaymobCallbackDto dto, CancellationToken ct)
        {
            try
            {
                // Validate payload & update payment in DB
                await _paymentService.HandlePaymobCallbackAsync(dto, ct);
                return Ok(new { Message = "Payment status updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Failed to process callback", Details = ex.Message });
            }
        }

        /// <summary>
        /// Optional: redirect user after payment
        /// </summary>
        [HttpGet("paymob/return")]
        [AllowAnonymous]
        public IActionResult PaymobReturn([FromQuery] string paymentId, [FromQuery] string success)
        {
            // You can show a "Payment Success" or "Payment Failed" page
            if (success == "true")
                return Redirect($"https://yourfrontend.com/payment-success?paymentId={paymentId}");

            return Redirect($"https://yourfrontend.com/payment-failed?paymentId={paymentId}");
        }
    }
}
    //    [HttpPost("confirm")]
    //    [Authorize(Roles = "Patient")]
    //    //public async Task<IActionResult> ConfirmPayment([FromBody] PaymobPaymentKeyResponse dto)
    //    //{
    //    //    try
    //    //    {
    //    //        var result = await _paymentService.ConfirmPaymentAsync(dto.token);
    //    //        return Ok(new { Success = result });
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        return StatusCode(500, new { Error = ex.Message });
    //    //    }
    //    //}

    //}

    // DTO for Paymob callback

//}
