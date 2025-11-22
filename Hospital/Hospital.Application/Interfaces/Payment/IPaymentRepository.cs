using Hospital.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Payment
{
    public interface IPaymentRepository
    {
        // Get appointment with related entities
        Task<Appointment?> GetAppointmentWithPatientAndDoctorAsync(int appointmentId, CancellationToken ct = default);

        // Add new payment
        Task AddPaymentAsync(Hospital.Domain.Models.Payment payment, CancellationToken ct = default);

        // Update existing payment
        Task UpdatePaymentAsync(Hospital.Domain.Models.Payment payment, CancellationToken ct = default);

        // Save changes
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        // Get payment by Paymob merchant order ID (for callback processing)
        Task<Hospital.Domain.Models.Payment?> GetPaymentByMerchantOrderIdAsync(string merchantOrderId, CancellationToken ct = default);

        // Get payment by Paymob transaction ID (optional)
        Task<Hospital.Domain.Models.Payment?> GetPaymentByTransactionIdAsync(long transactionId, CancellationToken ct = default);

    }
}
