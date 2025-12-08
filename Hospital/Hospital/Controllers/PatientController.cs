using Hospital.Application.DTO.Patient;
using Hospital.Application.Interfaces.Services;
using Hospital.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientController> _logger;

        public PatientController(IPatientService patientService, ILogger<PatientController> logger)
        {
            _patientService = patientService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Patient, Doctor,Admin")]

        public async Task<IActionResult> GetPatientById(int id)
        {
            _logger.LogInformation("get patient by id called at {time}", DateTime.Now);

            var patient = await _patientService.GetPatientByIdAsync(id);
            return Ok(patient);
        }

        [HttpGet]
        [Authorize(Roles = "Doctor,Admin")]

        public async Task<IActionResult> GetAllPatients()
        {
            _logger.LogInformation("get all patient  called at {time}", DateTime.Now);

            var patients = await _patientService.GetAllPatientsAsync();
            return Ok(patients);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UpdatePatient(UpdatePatientDto dto)
        {
            _logger.LogInformation("update patient  called at {time}", DateTime.Now);

            var updatedPatient = await _patientService.UpdatePatientAsync(dto);
            return Ok(updatedPatient);
        }
    }
}

