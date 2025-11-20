using Clinic.Infrastructure.Persistence;
using Hospital.Application.Interfaces.Payment;
using Hospital.Domain.Models;
using Hospital.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Application.DTO.PaymentDTOs;

namespace Hospital.Infrastructure.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymobClient _paymobClient;

        public PaymentService(IPaymentRepository paymentRepository, IPaymobClient paymobClient)
        {
            _paymentRepository = paymentRepository;
            _paymobClient = paymobClient;
        }

        public async Task<string> CreatePaymobPaymentForAppointmentAsync(
            int appointmentId,
            string currentUserId,
            CancellationToken ct = default)
        {
            // Load appointment
            var appointment = await _paymentRepository.GetAppointmentWithPatientAndDoctorAsync(appointmentId, ct);
            if (appointment == null)
                throw new InvalidOperationException("Appointment not found.");

            if (appointment.Payment != null)
                throw new InvalidOperationException("Payment already exists for this appointment.");

            if (appointment.Patient.User.Id != currentUserId)
                throw new UnauthorizedAccessException("Current user cannot pay for this appointment.");

            var user = appointment.Patient.User;

            // Create Payment entity
            var payment = new Hospital.Domain.Models.Payment
            {
                AppointmentId = appointment.AppointmentId,
                Amount = appointment.Doctor.ConsultationFees.Value,
                Currency = "EGP",
                Status = PaymentStatus.Pending,
            };

            await _paymentRepository.AddPaymentAsync(payment, ct);

            var merchantOrderId = payment.PaymentId.ToString();

            // Paymob: authenticate
            var authToken = await _paymobClient.AuthenticateAsync(ct);

            // Create Paymob order
            var paymobOrderId = await _paymobClient.CreateOrderAsync(
                authToken,
                payment.Amount,
                payment.Currency,
                merchantOrderId,
                ct);

            payment.PaymobOrderId = paymobOrderId;
            payment.PaymobMerchantOrderId = merchantOrderId;

            await _paymentRepository.UpdatePaymentAsync(payment, ct);

            // Generate Paymob payment key
            var paymentKey = await _paymobClient.GeneratePaymentKeyAsync(
                authToken,
                paymobOrderId,
                payment.Amount,
                payment.Currency,
                user.Email ?? "test@example.com",
                user.FullName,
                user.PhoneNumber ?? "NA",
                redirectUrl: "http://127.0.0.1:5500/payment-callback.html", // your callback page
                ct
);

            return paymentKey;
        }

        /// <summary>
        /// Handles Paymob webhook/callback to update payment status
        /// </summary>
        public async Task HandlePaymobCallbackAsync(PaymobCallbackDto dto, CancellationToken ct = default)
        {
            // 1️⃣ Load the payment by Paymob merchant order id
            var payment = await _paymentRepository.GetPaymentByMerchantOrderIdAsync(dto.OrderId, ct);
            if (payment == null)
                throw new InvalidOperationException("Payment not found for this merchant order.");

            // 2️⃣ Update the Paymob transaction id (convert string to long safely)
            if (long.TryParse(dto.PaymentId, out var transactionId))
            {
                payment.PaymobTransactionId = transactionId;
            }
            else
            {
                throw new InvalidOperationException("Invalid Paymob transaction ID.");
            }

            // 3️⃣ Map string status to enum
            payment.Status = dto.Status.ToUpper() switch
            {
                "CAPTURED" => Domain.Enum.PaymentStatus.Paid,
                "FAILED" => Domain.Enum.PaymentStatus.Failed,
                "PENDING" => Domain.Enum.PaymentStatus.Pending,
                _ => Domain.Enum.PaymentStatus.Pending
            };

            // 4️⃣ Update timestamps
            payment.UpdatedAt = DateTime.UtcNow;

            // 5️⃣ Save changes
            await _paymentRepository.UpdatePaymentAsync(payment, ct);
        }
        /// <summary>
        /// Confirms the payment after Paymob processing
        /// </summary>
        //public async Task<bool> ConfirmPaymentAsync(string paymentToken, CancellationToken ct = default)
        //{
        //    // 1️⃣ Validate the payment token with Paymob (you might need a Paymob endpoint for this)
        //    // For example, you can call Paymob API to check if the payment was successful
        //    var paymentStatus = await _paymobClient.CheckPaymentStatusAsync(paymentToken, ct);

        //    if (paymentStatus == null)
        //        throw new InvalidOperationException("Payment not found or token invalid.");

        //    // 2️⃣ Load the payment from your database
        //    // walaa look here
        //    //var payment = await _paymentRepository.GetPaymentByPaymobOrderIdAsync(paymentStatus.OrderId, ct);
        //    //if (payment == null)
        //    //    throw new InvalidOperationException("Payment record not found in database.");

        //    // 3️⃣ Update payment status
        //    payment.Status = paymentStatus.Success ? PaymentStatus.Paid : PaymentStatus.Failed;
        //    payment.PaymobTransactionId = paymentStatus.TransactionId;
        //    payment.UpdatedAt = DateTime.UtcNow;

        //    await _paymentRepository.UpdatePaymentAsync(payment, ct);

        //    return payment.Status == PaymentStatus.Paid;
        //}

    }

    // DTO for Paymob callback


}
