using Hospital.Application.DTO.PaymentDTOs;
using Hospital.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Payment
{
    public interface IPaymentService
    {
        /// <summary>
        /// Creates a Paymob payment for the given appointment, using the current user as customer.
        /// </summary>
        /// <param name="appointmentId">Appointment to pay for</param>
        /// <param name="currentUserId">The user making the payment</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Paymob payment token</returns>
        Task<string> CreatePaymobPaymentForAppointmentAsync(
            int appointmentId,
            string currentUserId,
            CancellationToken ct = default);

        /// <summary>
        /// Updates a payment status after receiving Paymob callback.
        /// </summary>
        /// <param name="merchantOrderId">Merchant order ID from Paymob</param>
        /// <param name="transactionId">Transaction ID from Paymob</param>
        /// <param name="status">Payment status string returned by Paymob</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Updated payment entity or null if not found</returns>

        Task HandlePaymobCallbackAsync(PaymobCallbackDto dto, CancellationToken ct = default);
    }
}

