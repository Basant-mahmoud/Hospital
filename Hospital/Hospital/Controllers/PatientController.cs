using Hospital.Application.DTO.Patient;
using Hospital.Application.Interfaces.Services;
using Hospital.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            return Ok(patient);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return Ok(patients);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePatient(UpdatePatientDto dto)
        {
            var updatedPatient = await _patientService.UpdatePatientAsync(dto);
            return Ok(updatedPatient);
        }
    }
}

