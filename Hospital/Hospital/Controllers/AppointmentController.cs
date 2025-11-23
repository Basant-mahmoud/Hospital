using Hospital.Application.DTO.Appointment;
using Hospital.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // ---------------------- Add Appointment ----------------------
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddAppointmentDto dto)
        {
            var created = await _appointmentService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.AppointmentId }, created);
        }

        // ---------------------- Get by ID ----------------------
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _appointmentService.GetByIdAsync(id);
            return Ok(result);
        }

        // ---------------------- Get by Doctor ----------------------
        [HttpGet("doctor/{doctorId:int}")]
        public async Task<IActionResult> GetByDoctorId(int doctorId)
        {
            var result = await _appointmentService.GetByDoctorId(doctorId);
            return Ok(result);
        }

        // ---------------------- Get by Patient ----------------------
        [HttpGet("patient/{patientId:int}")]
        public async Task<IActionResult> GetByPatientId(int patientId)
        {
            var result = await _appointmentService.GetByPatientId(patientId);
            return Ok(result);
        }

        // ---------------------- Delete Appointment ---------------------
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _appointmentService.DeleteAsync(id);
            return Ok(new { message = "Appointment deleted successfully." });
        }
    }
}
