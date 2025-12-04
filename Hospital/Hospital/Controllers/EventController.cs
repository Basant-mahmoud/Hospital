using Hospital.Application.DTO.Event;
using Hospital.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventController> _logger;

        public EventController(IEventService eventService, ILogger<EventController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        [HttpPost("CreateEvent")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateEvent([FromBody] AddEventDto eventDto)
        {
            _logger.LogInformation("create event  called at {time}", DateTime.Now);

            var createdEvent = await _eventService.AddAsync(eventDto);
            return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.EventId }, createdEvent);
        }

        [HttpPost("GetEvent")]

        public async Task<IActionResult> GetEvent(GetEventDto @event)
        {
            _logger.LogInformation("Get event  called at {time}", DateTime.Now);

            var eventDto = await _eventService.GetAsync(@event);
            if (eventDto == null)
                return NotFound();

            return Ok(eventDto);
        }


        [HttpGet("GetAllEvents")]
        public async Task<IActionResult> GetAllEvents([FromQuery] int branchId)
        {
            _logger.LogInformation("get all event  called at {time}", DateTime.Now);

            var events = await _eventService.GetAllAsync(branchId);
            if (events == null || !events.Any())
                return NotFound("No events found for this branch.");

            return Ok(events);
        }

        [HttpPut("UpdateEvent")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UpdateEvent([FromBody] EventDto eventDto)
        {
            _logger.LogInformation("update event  called at {time}", DateTime.Now);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _eventService.UpdateAsync(eventDto);
            if (result == 0)
                return NotFound($"No event found with ID = {eventDto.EventId}");

            return Ok("Event updated successfully.");
        }

        [HttpDelete("DeleteEvent")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteEvent(GetEventDto dto)
        {
            _logger.LogInformation("delete event  called at {time}", DateTime.Now);

            var result = await _eventService.DeleteAsync(dto);
            if (result == 0)
                return NotFound($"No event found with ID = {dto.EventId}");

            return Ok($"Event with ID = {dto.EventId} deleted successfully.");
        }

        [HttpGet("GetAllEventsInSystem")]
        [Authorize(Roles = "Admin, Patient")]

        public async Task<IActionResult> GetAllEventsInSystem()
        {
            _logger.LogInformation("get all  event in system  called at {time}", DateTime.Now);

            var events = await _eventService.GetAllEventInSystemAsync();
            if (events == null || !events.Any())
                return NotFound("No events found in the system.");
            return Ok(events);
        }

    }
}
