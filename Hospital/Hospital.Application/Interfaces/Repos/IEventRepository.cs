using Hospital.Application.DTO.Event;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Repos
{
    public interface IEventRepository
    {
        Task<Event> AddAsync(Event @event);          
        Task<int> UpdateAsync(Event @event);
        Task<int> DeleteAsync(Event @event);
        Task<Event?> GetAsync(int id);               
        Task<IEnumerable<Event>> GetAllAsync(int branchId);
        Task<IEnumerable<Event>> GetAllEventInSystemAsync();
    }
}
