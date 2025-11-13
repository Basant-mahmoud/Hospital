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
        private readonly IBranchService _branchService;
        private readonly IMapper _mapper;
        public EventService(IEventRepository eventRepository, IMapper mapper,IBranchService branchService)
        {
            _eventRepository = eventRepository;
            _branchService = branchService;
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
            var existingBranch = await _branchService.GetByIdAsync(eventdto.BranchId.Value);
            if (existingBranch == null)
                throw new KeyNotFoundException($"Branch with ID {eventdto.BranchId} not found.");
            var eventEntity = _mapper.Map<Event>(eventdto);
            eventEntity.CreatedAt = DateTime.UtcNow;
            eventEntity.UpdatedAt = DateTime.UtcNow;
            var savedEntity = await _eventRepository.AddAsync(eventEntity);

            if (savedEntity == null)
                throw new Exception("Failed to add the event.");

            return _mapper.Map<EventDto>(savedEntity);

        }

        public async Task<int> DeleteAsync(GetEventDto eventdto)
        {
            if (eventdto == null)
                throw new ArgumentNullException(nameof(eventdto), "Event cannot be null.");
            if(eventdto.EventId <= 0)
                throw new ArgumentException("Invalid event ID.", nameof(eventdto.EventId));
            if(eventdto.BranchId<=0)
                throw new ArgumentException("Invalid branch ID.", nameof(eventdto.BranchId));
            var existingBranch = await _branchService.GetByIdAsync(eventdto.BranchId);
            if (existingBranch == null)
                throw new KeyNotFoundException($"Branch with ID {eventdto.BranchId} not found.");
            var eventEntity = await _eventRepository.GetAsync(eventdto.EventId);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Event with ID {eventdto.EventId} not found.");
            var result = await _eventRepository.DeleteAsync(eventEntity);
            if (result == 0)
                throw new Exception("Failed to delete the event.");
            else
                return result;
        }

        public async Task<IEnumerable<AddEventDto>> GetAllAsync(int branchId)
        {
            var eventEntitys=await _eventRepository.GetAllAsync( branchId);
            if (eventEntitys == null || !eventEntitys.Any())
                return Enumerable.Empty<AddEventDto>();
            return _mapper.Map<IEnumerable<AddEventDto>>(eventEntitys);
        }

        public async Task<EventDto?> GetAsync(GetEventDto @event)
        {
            if(@event.BranchId<=0)
                throw new ArgumentNullException("Invalid branch ID.", nameof(@event.BranchId));
            var existingBranch = await _branchService.GetByIdAsync(@event.BranchId);
            if (existingBranch == null)
                throw new KeyNotFoundException($"Branch with ID {@event.BranchId} not found.");
            if (@event.EventId <= 0)
                throw new ArgumentException("Invalid event ID.", nameof(@event.EventId));
            var eventEntity = await _eventRepository.GetAsync(@event.EventId);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Event with ID {@event.EventId} not found.");
            return _mapper.Map<EventDto>(eventEntity);

        }

        public async Task<int> UpdateAsync(EventDto eventdto)
        {
           if(eventdto == null)
                throw new ArgumentNullException(nameof(eventdto), "Event cannot be null.");
            if(string.IsNullOrWhiteSpace(eventdto.Title))
                throw new ArgumentException("Event title cannot be empty.", nameof(eventdto.Title));
            if(eventdto.EventDate < DateTime.UtcNow)
                throw new ArgumentException("Event date cannot be in the past.", nameof(eventdto.EventDate));
            if(eventdto.BranchId==null)
                throw new ArgumentException("Branch ID cannot be null.", nameof(eventdto.BranchId));
            var existingBranch = await _branchService.GetByIdAsync(eventdto.BranchId.Value);
            if (existingBranch == null)
                throw new KeyNotFoundException($"Branch with ID {eventdto.BranchId} not found.");
            var eventEntity = _mapper.Map<Event>(eventdto);
            eventEntity.UpdatedAt = DateTime.UtcNow;
            return await _eventRepository.UpdateAsync(eventEntity);
        }
    }
}
