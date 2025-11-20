using Clinic.Infrastructure.Persistence;
using Hospital.Application.Interfaces.Payment;
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

        public async Task<Hospital.Domain.Models.Payment?> GetPaymentByMerchantOrderIdAsync(string merchantOrderId, CancellationToken ct = default)
        {
            return await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.PaymobMerchantOrderId == merchantOrderId, ct);
        }

        public async Task<Hospital.Domain.Models.Payment?> GetPaymentByTransactionIdAsync(long transactionId, CancellationToken ct = default)
        {
            return await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.PaymobTransactionId == transactionId, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
