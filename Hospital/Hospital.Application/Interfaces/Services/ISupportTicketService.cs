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
        // Patient creates a new ticket
        Task<SupportTicketDto> CreateAsync( CreateSupportTicketDto dto);

        // Admin updates a ticket (response or status)
        Task<SupportTicketDto> UpdateAsync(UpdateSupportTicketDto dto);

        // Admin or optional: delete a ticket
        Task<bool> DeleteAsync(int ticketId);

        // Get a ticket by id
        Task<SupportTicketDto?> GetByIdAsync(int ticketId);

        // Patient gets all of their tickets
        Task<IEnumerable<SupportTicketDto>> GetAllByUserAsync(string userId);

        // Admin gets all tickets
        Task<IEnumerable<SupportTicketDto>> GetAllAsync();
    }
}
