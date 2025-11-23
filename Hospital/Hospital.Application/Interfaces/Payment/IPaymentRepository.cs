using Hospital.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Payment
{
    public interface IPaymentRepository
    {

        Task<Appointment?> GetAppointmentWithPatientAndDoctorAsync(int appointmentId, CancellationToken ct = default);
        Task AddPaymentAsync(Hospital.Domain.Models.Payment payment, CancellationToken ct = default);
        Task UpdatePaymentAsync(Hospital.Domain.Models.Payment payment, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        // Get payment by Paymob merchant order ID (for callback processing)
        Task<Hospital.Domain.Models.Payment?> GetPaymentByMerchantOrderIdAsync(string merchantOrderId, CancellationToken ct = default);
        Task<Hospital.Domain.Models.Payment?> GetPaymentByTransactionIdAsync(long transactionId, CancellationToken ct = default);
        Task<Hospital.Domain.Models.Payment> CreatePendingPaymentAsync(int appointmentId, decimal amount, string currency = "EGP");
        Task<int> GetTodayCompletedForDoctorAsync(Hospital.Domain.Models.Payment payment);
        Task<Hospital.Domain.Models.Payment?> GetPaymentByAppointmentIdAsync(int appointmentId, CancellationToken ct = default);


    }
}
