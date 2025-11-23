using Clinic.Infrastructure.Persistence;
using Hospital.Application.Interfaces.Payment;
using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.PaymentRepAndService
{
    public class PaymentRepository : IPaymentRepository
    {

        private readonly AppDbContext _dbContext;
        public PaymentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Appointment?> GetAppointmentWithPatientAndDoctorAsync(int appointmentId, CancellationToken ct = default)
        {
            return await _dbContext.Appointments
                .Include(a => a.Payment)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User) 
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId, ct);
        }

        public async Task AddPaymentAsync(Hospital.Domain.Models.Payment payment, CancellationToken ct = default)
        {
            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task UpdatePaymentAsync(Hospital.Domain.Models.Payment payment, CancellationToken ct = default)
        {
            _dbContext.Payments.Update(payment);
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<Hospital.Domain.Models.Payment?> GetPaymentByMerchantOrderIdAsync(string merchantOrderId, CancellationToken ct = default)
        {
            return await _dbContext.Payments
                .Include(p => p.Appointment) 
                .FirstOrDefaultAsync(p => p.PaymobMerchantOrderId == merchantOrderId, ct);
        }

        public async Task<Hospital.Domain.Models.Payment?> GetPaymentByTransactionIdAsync(long transactionId, CancellationToken ct = default)
        {
            return await _dbContext.Payments
                .Include(p => p.Appointment)
                .FirstOrDefaultAsync(p => p.PaymobTransactionId == transactionId, ct);
        }

        public async Task<Hospital.Domain.Models.Payment> CreatePendingPaymentAsync(int appointmentId, decimal amount, string currency = "EGP")
        {
            var payment = new Hospital.Domain.Models.Payment
            {
                AppointmentId = appointmentId,
                Amount = amount,
                Currency = currency,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();

            return payment;
        }
         public async  Task<int> GetTodayCompletedForDoctorAsync(Hospital.Domain.Models.Payment payment)
        {
           _dbContext.Payments.Update(payment);
           return await _dbContext.SaveChangesAsync();
        }
        public async Task<Hospital.Domain.Models.Payment?> GetPaymentByAppointmentIdAsync(int appointmentId, CancellationToken ct = default)
        {
            return await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.AppointmentId == appointmentId, ct);
        }

    }
}

