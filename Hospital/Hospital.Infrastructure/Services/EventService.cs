using AutoMapper;
using Hospital.Application.DTO.Event;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        public EventService(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }
        public async Task<EventDto> AddAsync(AddEventDto eventdto)
        {
            
            if(string.IsNullOrWhiteSpace(eventdto.Title))
                throw new ArgumentException("Event title cannot be empty.", nameof(eventdto.Title));
            if(eventdto.EventDate < DateTime.UtcNow)
                throw new ArgumentException("Event date cannot be in the past.", nameof(eventdto.EventDate));
            if(eventdto.BranchId==null)
                throw new ArgumentException("Branch ID cannot be null.", nameof(eventdto.BranchId));
            // check that branch id exist in database or not call from branch 
            var eventEntity = _mapper.Map<Event>(eventdto);
            eventEntity.CreatedAt = DateTime.UtcNow;
            eventEntity.UpdatedAt = DateTime.UtcNow;
            var savedEntity = await _eventRepository.AddAsync(eventEntity);

            if (savedEntity == null)
                throw new Exception("Failed to add the event.");

            return _mapper.Map<EventDto>(savedEntity);

        }

        public Task<int> DeleteAsync(AddEventDto eventdto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AddEventDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<EventDto?> GetAsync(GetEvent @event)
        {
            //check brach is exist 
            if(@event.EventId <= 0)
                throw new ArgumentException("Invalid event ID.", nameof(@event.EventId));
            var eventEntity = await _eventRepository.GetAsync(@event.EventId);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Event with ID {@event.EventId} not found.");
            return _mapper.Map<EventDto>(eventEntity);

        }

        public Task<int> UpdateAsync(EventDto eventdto)
        {
            throw new NotImplementedException();
        }
    }
}
