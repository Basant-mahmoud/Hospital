using Hospital.Application.DTO.PaymentDTOs;
using Hospital.Application.Interfaces.Payment;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Payment
{
    public class PaymobClient : IPaymobClient
    {
        private readonly HttpClient _http;
        private readonly PaymobOptions _options;
        private const string BaseUrl = "https://accept.paymob.com/api";

        public PaymobClient(HttpClient http, IOptions<PaymobOptions> options)
        {
            _http = http;
            _options = options.Value;
        }

        public async Task<PaymobAuthResponse> AuthenticateAsync(CancellationToken ct = default)
        {
            var request = new PaymobAuthRequest { api_key = _options.ApiKey };
            var response = await _http.PostAsJsonAsync($"{BaseUrl}/auth/tokens", request, ct);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PaymobAuthResponse>(ct);
            return result ?? throw new InvalidOperationException("Failed to parse Paymob auth response.");
        }

        public async Task<PaymobOrderResponse> CreateOrderAsync(string authToken,decimal amount,string currency,
            string merchantOrderId,CancellationToken ct = default)
        {
            var amountCents = (int)(amount * 100);
            var request = new PaymobOrderRequest
            {
                auth_token = authToken,
                delivery_needed = "false",
                amount_cents = amountCents,
                currency = currency,
                merchant_order_id = merchantOrderId,
                items = Array.Empty<object>()
            };

            var response = await _http.PostAsJsonAsync($"{BaseUrl}/ecommerce/orders", request, ct);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PaymobOrderResponse>(ct);
            return result ?? throw new InvalidOperationException("Failed to parse Paymob order response.");
        }

        public async Task<PaymobPaymentKeyResponse> GeneratePaymentKeyAsync(string authToken,long paymobOrderId,decimal amount,string currency,
            string customerEmail,string fullName, string phone, string redirectUrl = null,CancellationToken ct = default)
        {
            var amountCents = (int)(amount * 100);
            var names = fullName.Split(' ', 2);
            var firstName = names.Length > 0 ? names[0] : "Customer";
            var lastName = names.Length > 1 ? names[1] : "User";

            var billingData = new BillingData
            {
                first_name = firstName,
                last_name = lastName,
                email = customerEmail,
                phone_number = phone
            };

            var request = new PaymobPaymentKeyRequest
            {
                auth_token = authToken,
                amount_cents = amountCents,
                currency = currency,
                order_id = paymobOrderId,
                integration_id = _options.CardIntegrationId,
                billing_data = billingData
            };

            var response = await _http.PostAsJsonAsync($"{BaseUrl}/acceptance/payment_keys", request, ct);
            response.EnsureSuccessStatusCode();

            PaymobPaymentKeyResponse? result = await response.Content.ReadFromJsonAsync<PaymobPaymentKeyResponse>(ct);
            return result ?? throw new InvalidOperationException("Failed to parse Paymob payment key response.");
        }

        public async Task<PaymobPaymentStatusDto> CheckPaymentStatusAsync(string paymentToken,CancellationToken ct = default)
        {
            // This would require additional Paymob API endpoint
            // For now, return a basic implementation
            throw new NotImplementedException("Payment status check not yet implemented.");
        }
    }
}
