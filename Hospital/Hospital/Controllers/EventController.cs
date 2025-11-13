using Hospital.Application.DTO.Event;
using Hospital.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }
        [HttpPost("CreateEvent")]
        public async Task<IActionResult> CreateEvent([FromBody] AddEventDto eventDto)
        {
            var createdEvent = await _eventService.AddAsync(eventDto);
            return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.EventId }, createdEvent);
        }


        [HttpPost("GetEvent")]
        public async Task<IActionResult> GetEvent(GetEventDto @event)
        {
            var eventDto = await _eventService.GetAsync(@event);
            if (eventDto == null)
                return NotFound();

            return Ok(eventDto);
        }
        [HttpGet("GetAllEvents")]
        public async Task<IActionResult> GetAllEvents([FromQuery] int branchId)
        {
            var events = await _eventService.GetAllAsync(branchId);
            if (events == null || !events.Any())
                return NotFound("No events found for this branch.");

            return Ok(events);
        }

        [HttpPut("UpdateEvent")]
        public async Task<IActionResult> UpdateEvent([FromBody] EventDto eventDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _eventService.UpdateAsync(eventDto);
            if (result == 0)
                return NotFound($"No event found with ID = {eventDto.EventId}");

            return Ok("Event updated successfully.");
        }

        [HttpDelete("DeleteEvent")]
        public async Task<IActionResult> DeleteEvent(GetEventDto dto)
        {
            var result = await _eventService.DeleteAsync(dto);
            if (result == 0)
                return NotFound($"No event found with ID = {dto.EventId}");

            return Ok($"Event with ID = {dto.EventId} deleted successfully.");
        }

    }
}
