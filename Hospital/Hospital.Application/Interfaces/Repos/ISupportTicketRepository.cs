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
        Task<SupportTicket> AddAsync(SupportTicket ticket); 
        Task<SupportTicket?> GetByIdAsync(int ticketId);    
        Task<IEnumerable<SupportTicket>> GetAllAsync();     
        Task<IEnumerable<SupportTicket>> GetAllByPatientIdAsync(string patientId);
        Task<int> UpdateAsync(SupportTicket ticket);       
        Task<int> DeleteAsync(int ticketId);               
    }
}
