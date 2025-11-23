using Hospital.Application.DTO.SupportTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Services
{
    public interface ISupportTicketService
    {
        Task<SupportTicketDto> CreateAsync( CreateSupportTicketDto dto);
        Task<SupportTicketDto> UpdateAsync(UpdateSupportTicketDto dto);
        Task<bool> DeleteAsync(int ticketId);
        Task<SupportTicketDto?> GetByIdAsync(int ticketId);
        Task<IEnumerable<SupportTicketDto>> GetAllByUserAsync(string userId);
        Task<IEnumerable<SupportTicketDto>> GetAllAsync();
    }
}
