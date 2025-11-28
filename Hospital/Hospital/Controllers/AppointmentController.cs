using Hospital.Application.DTO.Appointment;
using Hospital.Application.Interfaces.Services;
using Hospital.Controllers;
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
        public async Task<IActionResult> Add([FromBody] AddAppointmentDto dto)
        {
            _logger.LogInformation("Add Appointment called at {time}", DateTime.Now);

            var created = await _appointmentService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.AppointmentId }, created);
        }

        // ---------------------- Get by ID ----------------------
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Get Appointment ByID called at {time}", DateTime.Now);

            var result = await _appointmentService.GetByIdAsync(id);
            return Ok(result);
        }

        // ---------------------- Get by Doctor ----------------------
        [HttpGet("doctor/{doctorId:int}")]
        public async Task<IActionResult> GetByDoctorId(int doctorId)
        {
            _logger.LogInformation("Get Appointment by GetByDoctorId  called at {time}", DateTime.Now);

            var result = await _appointmentService.GetByDoctorId(doctorId);
            return Ok(result);
        }

        // ---------------------- Get by Patient ----------------------
        [HttpGet("patient/{patientId:int}")]
        public async Task<IActionResult> GetByPatientId(int patientId)
        {
            _logger.LogInformation("Get Appointment by GetByPatientId  called at {time}", DateTime.Now);

            var result = await _appointmentService.GetByPatientId(patientId);
            return Ok(result);
        }

        // ---------------------- Delete Appointment ---------------------
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Delete Appointment called at {time}", DateTime.Now);

            await _appointmentService.DeleteAsync(id);
            return Ok(new { message = "Appointment deleted successfully." });
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Get All Appointments called at {time}", DateTime.Now);
            var result = await _appointmentService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("GetAllCompleted")]
        public async Task<IActionResult> GetCompletedAppointment()
        {
            _logger.LogInformation("Get All Completed Appointments called at {time}", DateTime.Now);
            var result = await _appointmentService.GetAllCompletedAsync();
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
        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            _logger.LogInformation("Mark Appointment as Completed called at {time}", DateTime.Now);
            var result = await _appointmentService.MarkAsCompletedAsync(id);
            
            return Ok(result);
        }


    }
}
