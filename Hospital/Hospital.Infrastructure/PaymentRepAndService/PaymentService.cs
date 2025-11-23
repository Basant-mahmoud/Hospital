using Hospital.Application.DTO.PaymentDTOs;
using Hospital.Application.Interfaces.Payment;
using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymobClient _paymobClient;
        private readonly PaymobOptions _options;

        public PaymentService(IPaymentRepository paymentRepository, IPaymobClient paymobClient, IOptions<PaymobOptions> options)
        {
            _paymentRepository = paymentRepository;
            _paymobClient = paymobClient;
            _options = options.Value;
        }

        public async Task<string> CreatePaymobPaymentForAppointmentAsync(
            int appointmentId,
            string currentUserId,
            CancellationToken ct = default)
        {
            // Load appointment
            var appointment = await _paymentRepository.GetAppointmentWithPatientAndDoctorAsync(appointmentId, ct);
            if (appointment == null)
                throw new InvalidOperationException($"Appointment {appointmentId} not found.");

            // Verify user owns this appointment
            if (appointment.Patient.UserId != currentUserId)
                throw new UnauthorizedAccessException("You can only pay for your own appointments.");

            // Check if already paid
            if (appointment.Payment != null && appointment.Payment.Status == PaymentStatus.Paid)
                throw new InvalidOperationException("This appointment has already been paid for.");

            // Get consultation fee
            var amount = appointment.Doctor.ConsultationFees;

            // Step 1: Authenticate with Paymob
            var authResponse = await _paymobClient.AuthenticateAsync(ct);
            var authToken = authResponse;

            // Step 2: Create Paymob order
            var merchantOrderId = $"APT-{appointmentId}-{DateTime.UtcNow.Ticks}";
            var orderResponse = await _paymobClient.CreateOrderAsync(
                authToken.token,
                amount,
                "EGP",
                merchantOrderId,
                ct);

            // Step 3: Generate payment key
            var paymentKeyResponse = await _paymobClient.GeneratePaymentKeyAsync(
                authToken.token,
                orderResponse.id,
                amount,
                "EGP",
                appointment.Patient.User.Email,
                $"{appointment.Patient.User.FullName}",
                appointment.Patient.User.PhoneNumber,
                redirectUrl: null, // You can add your frontend callback URL here
                ct);

            // Step 4: Create Payment record in DB
            var payment = new Hospital.Domain.Models.Payment
            {
                AppointmentId = appointmentId,
                Amount = amount,
                Currency = "EGP",
                PaymobOrderId = orderResponse.id,
                PaymobMerchantOrderId = merchantOrderId,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddPaymentAsync(payment, ct);

            // Return payment token for frontend
            return paymentKeyResponse.token;
        }

        public async Task<Hospital.Domain.Models.Payment?> HandlePaymobCallbackAsync(PaymobCallbackDto dto,CancellationToken ct = default)
        {
            // Find payment by merchant order ID
            var payment = await _paymentRepository.GetPaymentByMerchantOrderIdAsync(dto.OrderId, ct);
            if (payment == null)
                return null; 

            // Prevent duplicate processing
            if (payment.Status == PaymentStatus.Paid)
                return payment; 

            // Update payment with Paymob data
            payment.PaymobTransactionId = long.TryParse(dto.PaymentId, out var txId) ? txId : (long?)null;
            payment.GatewayResponse = JsonSerializer.Serialize(dto);
            payment.UpdatedAt = DateTime.UtcNow;

            // Map Paymob status to our enum
            payment.Status = MapPaymobStatus(dto.Status);

            // Update in database
            await _paymentRepository.UpdatePaymentAsync(payment, ct);

            return payment;
        }

        private PaymentStatus MapPaymobStatus(string paymobStatus)
        {
            return paymobStatus?.ToLowerInvariant() switch
            {
                "success" => PaymentStatus.Paid,
                "paid" => PaymentStatus.Paid,
                "true" => PaymentStatus.Paid,
                "failed" => PaymentStatus.Failed,
                "false" => PaymentStatus.Failed,
                "pending" => PaymentStatus.Pending,
                "cancelled" => PaymentStatus.Cancelled,
                "canceled" => PaymentStatus.Cancelled,
                _ => PaymentStatus.Failed
            };
        }
    }
}
