using Hospital.Application.DTO.PaymentDTOs;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Payment
{
    public interface IPaymobClient
    {
        Task<PaymobAuthResponse> AuthenticateAsync(CancellationToken ct = default);

        Task<PaymobOrderResponse> CreateOrderAsync(
            string authToken,
            decimal amount,
            string currency,
            string merchantOrderId,
            CancellationToken ct = default);

        /// <summary>
        /// Generates a Paymob payment key with optional redirect URL for post-payment callback.
        /// </summary>
        Task<PaymobPaymentKeyResponse> GeneratePaymentKeyAsync(
            string authToken,
            long paymobOrderId,
            decimal amount,
            string currency,
            string customerEmail,
            string fullName,
            string phone,
            string redirectUrl = null,
            CancellationToken ct = default);

        Task<PaymobPaymentStatusDto> CheckPaymentStatusAsync(
            string paymentToken,
            CancellationToken ct = default);
    }
}
