using Hospital.Application.DTO.Schedule;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Enum;
using Hospital.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _service;
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(IScheduleService service, ILogger<ScheduleController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // Create a new schedule
        [HttpPost]
        public async Task<ActionResult<ScheduleDto>> Create([FromBody] CreateScheduleDto dto)
        {
            _logger.LogInformation("Create schedule called at {time}", DateTime.Now);
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        // Update existing schedule
        [HttpPut]
        public async Task<ActionResult<ScheduleDto>> Update([FromBody] UpdateScheduleDto dto)
        {
            _logger.LogInformation("Update schedule called at {time}", DateTime.Now);
            var result = await _service.UpdateAsync(dto);
            return Ok(result);
        }

        // Delete schedule
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("Delete schedule called at {time}", DateTime.Now);
            var deleted = await _service.DeleteAsync(id);
            return deleted ? Ok("Deleted successfully") : NotFound();
        }

        // Get schedule by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ScheduleDto>> GetById(int id)
        {
            _logger.LogInformation("Get schedule by ID called at {time}", DateTime.Now);
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        // Get all schedules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetAll()
        {
            _logger.LogInformation("Get all schedules called at {time}", DateTime.Now);
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // Get schedules by doctor ID
        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetByDoctorId(int doctorId)
        {
            _logger.LogInformation("Get schedules by doctor ID called at {time}", DateTime.Now);

            if (doctorId <= 0)
                return BadRequest("Invalid doctor ID.");

            var schedules = await _service.GetSchedulesByDoctorIdAsync(doctorId);
            if (schedules == null || !schedules.Any())
                return NotFound($"No schedules found for doctor ID {doctorId}.");

            return Ok(schedules);
        }

        // Get schedules by day name (e.g., "Monday")
        [HttpGet("day/{dayOfWeek}")]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetDoctorsByDay(string dayOfWeek)
        {
            _logger.LogInformation("Get schedules by day called at {time}", DateTime.Now);
            var result = await _service.GetDoctorsByDateAsync(dayOfWeek);
            return Ok(result);
        }

        // Get schedules by exact date
        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetDoctorsByDate(DateOnly date)
        {
            _logger.LogInformation("Get schedules by date called at {time}", DateTime.Now);

            if (date < DateOnly.FromDateTime(DateTime.Today))
                return BadRequest("Cannot get schedules for a past date.");

            var schedules = await _service.GetSchedulesByDateOnlyAsync(date);
            return Ok(schedules);
        }

        // Get schedules by day name and shift
        [HttpGet("day/{dayOfWeek}/shift/{shift}")]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetDoctorsByDayAndShift(string dayOfWeek, AppointmentShift shift)
        {
            _logger.LogInformation("Get schedules by day and shift called at {time}", DateTime.Now);
            var result = await _service.GetDoctorsByDateAndShiftAsync(dayOfWeek, shift);
            return Ok(result);
        }

        // Get schedules by date and shift
        [HttpGet("date/{date}/shift/{shift}")]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetDoctorsByDateAndShift(DateOnly date, AppointmentShift shift)
        {
            _logger.LogInformation("Get schedules by date and shift called at {time}", DateTime.Now);

            if (date < DateOnly.FromDateTime(DateTime.Today))
                return BadRequest("Cannot get schedules for a past date.");

            var dayOfWeek = date.DayOfWeek.ToString();
            var result = await _service.GetDoctorsByDateAndShiftAsync(dayOfWeek, shift);
            return Ok(result);
        }
    }
}
