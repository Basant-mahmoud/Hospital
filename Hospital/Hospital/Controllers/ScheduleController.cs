using Hospital.Application.DTO.Schedule;
using Hospital.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        // GET: api/Schedule
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var schedules = await _scheduleService.GetAllAsync();
            return Ok(schedules);
        }

        // GET: api/Schedule/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var schedule = await _scheduleService.GetByIdAsync(id);
            return schedule == null ? NotFound() : Ok(schedule);
        }

        // GET: api/Schedule/doctor/5
        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetAllByDoctorId(int doctorId)
        {
            var schedules = await _scheduleService.GetAllByDoctorIdAsync(doctorId);
            return Ok(schedules);
        }

        // POST: api/Schedule
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateScheduleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _scheduleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ScheduleId }, created);
        }

        // PUT: api/Schedule/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateScheduleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.ScheduleId) return BadRequest("ID mismatch");

            await _scheduleService.UpdateAsync(dto);
            return NoContent();
        }

        // DELETE: api/Schedule/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _scheduleService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}

