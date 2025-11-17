using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Repos
{
    public interface ISupportTicketRepository
    {
        Task<SupportTicket> AddAsync(SupportTicket ticket); // patient creates
        Task<SupportTicket?> GetByIdAsync(int ticketId);    // get one
        Task<IEnumerable<SupportTicket>> GetAllAsync();     // all tickets (admin)
        Task<IEnumerable<SupportTicket>> GetAllByPatientIdAsync(string patientId); // patient and admin
        Task<int> UpdateAsync(SupportTicket ticket);        // admin only
        Task<int> DeleteAsync(int ticketId);               // admin only
    }
}
