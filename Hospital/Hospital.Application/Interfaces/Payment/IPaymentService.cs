using Hospital.Application.DTO.PaymentDTOs;
using Hospital.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Payment
{
    public interface IPaymentService
    {
        Task<string> CreatePaymobPaymentForAppointmentAsync(int appointmentId,string currentUserId,CancellationToken ct = default);
        Task<Hospital.Domain.Models.Payment?> HandlePaymobCallbackAsync(PaymobCallbackDto dto, CancellationToken ct = default);
    }
}

