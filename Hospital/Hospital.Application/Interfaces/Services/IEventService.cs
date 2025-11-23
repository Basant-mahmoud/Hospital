using Hospital.Application.DTO.Event;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Services
{
    public  interface IEventService
    {
        Task<EventDto> AddAsync(AddEventDto @eventdto);
        Task<int> UpdateAsync(EventDto @eventdto);
        Task<int> DeleteAsync(GetEventDto eventdto);
        Task<EventDto?> GetAsync(GetEventDto @event);
        Task<IEnumerable<EventDto>> GetAllAsync(int branchId);
        Task<IEnumerable<EventDto>> GetAllEventInSystemAsync();
    }
}
