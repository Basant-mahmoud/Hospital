using Hospital.Application.DTO.PaymentDTOs;
using Hospital.Application.Interfaces.Payment;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
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

        public async Task<string> AuthenticateAsync(CancellationToken ct = default)
        {
            var request = new PaymobAuthRequest { api_key = _options.ApiKey };
            var response = await _http.PostAsJsonAsync($"{BaseUrl}/auth/tokens", request, ct);
            var raw = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Paymob Authenticate Error: {response.StatusCode}, Body: {raw}");

            var body = JsonSerializer.Deserialize<PaymobAuthResponse>(raw);
            if (body == null || string.IsNullOrWhiteSpace(body.token))
                throw new InvalidOperationException("Failed to get Paymob auth token.");

            return body.token;
        }

        public async Task<long> CreateOrderAsync(
            string authToken,
            decimal amount,
            string currency,
            string merchantOrderId,
            CancellationToken ct = default)
        {
            var request = new PaymobOrderRequest
            {
                auth_token = authToken,
                amount_cents = (int)(amount * 100m),
                currency = currency,
                merchant_order_id = merchantOrderId
            };

            var response = await _http.PostAsJsonAsync($"{BaseUrl}/ecommerce/orders", request, ct);
            var raw = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Paymob CreateOrder Error: {response.StatusCode}, Body: {raw}");

            var body = JsonSerializer.Deserialize<PaymobOrderResponse>(raw);
            if (body == null || body.id <= 0)
                throw new InvalidOperationException("Failed to create Paymob order.");

            return body.id;
        }

        public async Task<string> GeneratePaymentKeyAsync(
            string authToken,
            long paymobOrderId,
            decimal amount,
            string currency,
            string customerEmail,
            string fullName,
            string phone,
            string redirectUrl = null,
            CancellationToken ct = default)
        {
            var names = fullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var firstName = names.FirstOrDefault() ?? "NA";
            var lastName = names.Length > 1 ? names.Last() : firstName;

            var request = new PaymobPaymentKeyRequest
            {
                auth_token = authToken,
                amount_cents = (int)(amount * 100m),
                currency = currency,
                order_id = paymobOrderId,
                integration_id = _options.CardIntegrationId,
                billing_data = new BillingData
                {
                    first_name = firstName,
                    last_name = lastName,
                    email = customerEmail,
                    phone_number = phone,
                    apartment = "NA",
                    floor = "NA",
                    street = "NA",
                    building = "NA",
                    shipping_method = "NA",
                    postal_code = "00000",
                    city = "Cairo",
                    country = "EG",
                    state = "Cairo"
                },

            };

            var response = await _http.PostAsJsonAsync($"{BaseUrl}/acceptance/payment_keys", request, ct);
            var raw = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Paymob PaymentKey Error: {response.StatusCode}, Body: {raw}");

            var body = JsonSerializer.Deserialize<PaymobPaymentKeyResponse>(raw);
            if (body == null || string.IsNullOrWhiteSpace(body.token))
                throw new InvalidOperationException("Failed to generate Paymob payment key.");

            return body.token;
        }

        public async Task<PaymobPaymentStatusDto> CheckPaymentStatusAsync(string paymentToken, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(paymentToken))
                throw new ArgumentException("Payment token is required.", nameof(paymentToken));

            var url = $"https://accept.paymobsolutions.com/api/acceptance/payment_keys/{paymentToken}";
            var response = await _http.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Paymob API error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var success = root.GetProperty("success").GetBoolean();
            var orderId = root.GetProperty("order").GetProperty("id").GetInt64();
            var transactionId = root.TryGetProperty("transaction", out var txn) && txn.TryGetProperty("id", out var txnId)
                ? txnId.GetString()
                : string.Empty;

            return new PaymobPaymentStatusDto
            {
                OrderId = orderId,
                TransactionId = transactionId ?? string.Empty,
                Success = success
            };
        }
    }
}
