using Clinic.Infrastructure.Persistence;
using Hospital.Application.Interfaces.Repos;
using Hospital.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Repository
{
    public class SupportTicketRepository : ISupportTicketRepository
    {

        private readonly AppDbContext _context;
        public SupportTicketRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SupportTicket> AddAsync(SupportTicket ticket)
        {
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SupportTickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<SupportTicket?> GetByIdAsync(int ticketId)
        {
            return await _context.SupportTickets
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);
        }

        public async Task<IEnumerable<SupportTicket>> GetAllAsync()
        {
            return await _context.SupportTickets
                .Include(t => t.User)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SupportTicket>> GetAllByPatientIdAsync(string patientId)
        {
            return await _context.SupportTickets
                .Where(t => t.UserId == patientId)
                .Include(t => t.User)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> UpdateAsync(SupportTicket ticket)
        {
            ticket.UpdatedAt = DateTime.UtcNow;
            _context.SupportTickets.Update(ticket);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int ticketId)
        {
            var ticket = await _context.SupportTickets.FindAsync(ticketId);
            if (ticket == null) return 0;

            _context.SupportTickets.Remove(ticket);
            return await _context.SaveChangesAsync();
        }
    }
}
