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


        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(GetEvent @event)
        {
            var eventDto = await _eventService.GetAsync(@event);
            if (eventDto == null)
                return NotFound();

            return Ok(eventDto);
        }
    }
}
