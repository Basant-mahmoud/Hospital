using Hospital.Application.DTO.Appointment;
using Hospital.Application.Interfaces.Services;
using Hospital.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(IAppointmentService appointmentService, ILogger<AppointmentController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        // ---------------------- Add Appointment ----------------------
        [HttpPost]
        [Authorize(Roles ="Patient")]
        public async Task<IActionResult> Add([FromBody] AddAppointmentDto dto)
        {
            _logger.LogInformation("Add Appointment called at {time}", DateTime.Now);

            var created = await _appointmentService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.AppointmentId }, created);
        }

        // ---------------------- Get by ID ----------------------
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Patient,Admin,Doctor")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Get Appointment ByID called at {time}", DateTime.Now);

            var result = await _appointmentService.GetByIdAsync(id);
            return Ok(result);
        }

        // ---------------------- Get by Doctor ----------------------
        [HttpGet("doctor/{doctorId:int}")]
        [Authorize(Roles = "Admin,Doctor")]

        public async Task<IActionResult> GetByDoctorId(int doctorId)
        {
            _logger.LogInformation("Get Appointment by GetByDoctorId  called at {time}", DateTime.Now);

            var result = await _appointmentService.GetByDoctorId(doctorId);
            return Ok(result);
        }

        // ---------------------- Get by Patient ----------------------
        [HttpGet("patient/{patientId:int}")]
        [Authorize(Roles = "Patient,Admin,Doctor")]
        public async Task<IActionResult> GetByPatientId(int patientId)
        {
            _logger.LogInformation("Get Appointment by GetByPatientId  called at {time}", DateTime.Now);

            var result = await _appointmentService.GetByPatientId(patientId);
            return Ok(result);
        }

        // ---------------------- Delete Appointment ---------------------
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Patient,Admin,Doctor")]

        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Delete Appointment called at {time}", DateTime.Now);

            await _appointmentService.DeleteAsync(id);
            return Ok(new { message = "Appointment deleted successfully." });
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,Doctor")]

        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Get All Appointments called at {time}", DateTime.Now);
            var result = await _appointmentService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("CanceledByDoctor/{doctorId}")]
        public async Task<IActionResult> GetAllAppointmentCancelByDoctorId(int doctorId)
        {
            _logger.LogInformation("Fetching canceled appointments for DoctorId: {DoctorId}", doctorId);
            var appointments = await _appointmentService.GetAllAppointmentCancelByDoctorId(doctorId);

            return Ok(appointments);
        }

        [HttpPut("MarkAsCompleted/{id:int}")]
        [Authorize(Roles = "Doctor")]

        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            _logger.LogInformation("Mark Appointment as Completed called at {time}", DateTime.Now);
            var result = await _appointmentService.MarkAsCompletedAsync(id);
            
            return Ok(result);
        }


    }
}
