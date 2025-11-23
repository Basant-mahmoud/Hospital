using AutoMapper;
using Hospital.Application.DTO.Event;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IBranchService _branchService;
        private readonly IMapper _mapper;
        private readonly ILogger<EventService> _logger;

        public EventService(IEventRepository eventRepository, IMapper mapper, IBranchService branchService, ILogger<EventService> logger)
        {
            _eventRepository = eventRepository;
            _branchService = branchService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<EventDto> AddAsync(AddEventDto eventdto)
        {
            _logger.LogInformation("Adding new event: {Title} for Branch ID: {BranchId}", eventdto.Title, eventdto.BranchId);

            if (string.IsNullOrWhiteSpace(eventdto.Title))
                throw new ArgumentException("Event title cannot be empty.", nameof(eventdto.Title));
            if (eventdto.EventDate < DateTime.UtcNow)
                throw new ArgumentException("Event date cannot be in the past.", nameof(eventdto.EventDate));
            if (eventdto.BranchId == null)
                throw new ArgumentException("Branch ID cannot be null.", nameof(eventdto.BranchId));

            var existingBranch = await _branchService.GetByIdAsync(eventdto.BranchId.Value);
            if (existingBranch == null)
            {
                _logger.LogWarning("Branch with ID {BranchId} not found", eventdto.BranchId);
                throw new KeyNotFoundException($"Branch with ID {eventdto.BranchId} not found.");
            }

            var eventEntity = _mapper.Map<Event>(eventdto);
            eventEntity.CreatedAt = DateTime.UtcNow;
            eventEntity.UpdatedAt = DateTime.UtcNow;

            var savedEntity = await _eventRepository.AddAsync(eventEntity);
            if (savedEntity == null)
            {
                _logger.LogError("Failed to add the event: {Title}", eventdto.Title);
                throw new Exception("Failed to add the event.");
            }

            _logger.LogInformation("Event {Title} added successfully with ID {EventId}", eventdto.Title, savedEntity.EventId);
            return _mapper.Map<EventDto>(savedEntity);
        }

        public async Task<int> DeleteAsync(GetEventDto eventdto)
        {
            _logger.LogInformation("Deleting event ID {EventId} for Branch ID {BranchId}", eventdto.EventId, eventdto.BranchId);

            if (eventdto == null)
                throw new ArgumentNullException(nameof(eventdto), "Event cannot be null.");
            if (eventdto.EventId <= 0)
                throw new ArgumentException("Invalid event ID.", nameof(eventdto.EventId));
            if (eventdto.BranchId <= 0)
                throw new ArgumentException("Invalid branch ID.", nameof(eventdto.BranchId));

            var existingBranch = await _branchService.GetByIdAsync(eventdto.BranchId);
            if (existingBranch == null)
            {
                _logger.LogWarning("Branch with ID {BranchId} not found", eventdto.BranchId);
                throw new KeyNotFoundException($"Branch with ID {eventdto.BranchId} not found.");
            }

            var eventEntity = await _eventRepository.GetAsync(eventdto.EventId);
            if (eventEntity == null)
            {
                _logger.LogWarning("Event with ID {EventId} not found", eventdto.EventId);
                throw new KeyNotFoundException($"Event with ID {eventdto.EventId} not found.");
            }

            var result = await _eventRepository.DeleteAsync(eventEntity);
            if (result == 0)
            {
                _logger.LogError("Failed to delete event ID {EventId}", eventdto.EventId);
                throw new Exception("Failed to delete the event.");
            }

            _logger.LogInformation("Event ID {EventId} deleted successfully", eventdto.EventId);
            return result;
        }

        public async Task<IEnumerable<EventDto>> GetAllAsync(int branchId)
        {
            _logger.LogInformation("Fetching all events for Branch ID {BranchId}", branchId);

            var eventEntities = await _eventRepository.GetAllAsync(branchId);
            if (eventEntities == null || !eventEntities.Any())
            {
                _logger.LogInformation("No events found for Branch ID {BranchId}", branchId);
                return Enumerable.Empty<EventDto>();
            }

            return _mapper.Map<IEnumerable<EventDto>>(eventEntities);
        }

        public async Task<EventDto?> GetAsync(GetEventDto @event)
        {
            _logger.LogInformation("Fetching event ID {EventId} for Branch ID {BranchId}", @event.EventId, @event.BranchId);

            if (@event.BranchId <= 0)
                throw new ArgumentNullException("Invalid branch ID.", nameof(@event.BranchId));
            var existingBranch = await _branchService.GetByIdAsync(@event.BranchId);
            if (existingBranch == null)
            {
                _logger.LogWarning("Branch with ID {BranchId} not found", @event.BranchId);
                throw new KeyNotFoundException($"Branch with ID {@event.BranchId} not found.");
            }

            if (@event.EventId <= 0)
                throw new ArgumentException("Invalid event ID.", nameof(@event.EventId));

            var eventEntity = await _eventRepository.GetAsync(@event.EventId);
            if (eventEntity == null)
            {
                _logger.LogWarning("Event with ID {EventId} not found", @event.EventId);
                throw new KeyNotFoundException($"Event with ID {@event.EventId} not found.");
            }

            return _mapper.Map<EventDto>(eventEntity);
        }

        public async Task<int> UpdateAsync(EventDto eventdto)
        {
            _logger.LogInformation("Updating event ID {EventId} for Branch ID {BranchId}", eventdto.EventId, eventdto.BranchId);

            if (eventdto == null)
                throw new ArgumentNullException(nameof(eventdto), "Event cannot be null.");
            if (string.IsNullOrWhiteSpace(eventdto.Title))
                throw new ArgumentException("Event title cannot be empty.", nameof(eventdto.Title));
            if (eventdto.EventDate < DateTime.UtcNow)
                throw new ArgumentException("Event date cannot be in the past.", nameof(eventdto.EventDate));
            if (eventdto.BranchId == null)
                throw new ArgumentException("Branch ID cannot be null.", nameof(eventdto.BranchId));

            var existingBranch = await _branchService.GetByIdAsync(eventdto.BranchId.Value);
            if (existingBranch == null)
            {
                _logger.LogWarning("Branch with ID {BranchId} not found", eventdto.BranchId);
                throw new KeyNotFoundException($"Branch with ID {eventdto.BranchId} not found.");
            }

            var eventEntity = _mapper.Map<Event>(eventdto);
            eventEntity.UpdatedAt = DateTime.UtcNow;

            var updated = await _eventRepository.UpdateAsync(eventEntity);
            _logger.LogInformation("Event ID {EventId} updated successfully", eventdto.EventId);

            return updated;
        }

        public async Task<IEnumerable<EventDto>> GetAllEventInSystemAsync()
        {
            _logger.LogInformation("Fetching all events in the system");

            var events = await _eventRepository.GetAllEventInSystemAsync();
            if (events == null || !events.Any())
            {
                _logger.LogInformation("No events found in the system");
                return Enumerable.Empty<EventDto>();
            }

            return _mapper.Map<IEnumerable<EventDto>>(events);
        }
    }
}
