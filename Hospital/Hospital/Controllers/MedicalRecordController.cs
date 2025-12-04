using Hospital.Application.DTO.MedicalRecord;
using Hospital.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly ILogger<MedicalRecordController> _logger;

        public MedicalRecordController(IMedicalRecordService medicalRecordService, ILogger<MedicalRecordController> logger)
        {
            _medicalRecordService = medicalRecordService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]

        public async Task<ActionResult<PatientMedicalRecordDto>> Add([FromBody] AddMedicalRecordDto dto)
        {
            _logger.LogInformation("add medical record  called at {time}", DateTime.Now);

            var result = await _medicalRecordService.AddAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Doctor")]

        public async Task<ActionResult<int>> Update([FromBody] UpdateMedicalRecordDto dto)
        {
            _logger.LogInformation("Updating medical record with ID {RecordId}", dto.RecordId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedDto = await _medicalRecordService.UpdateAsync(dto);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor")]

        public async Task<ActionResult<int>> Delete(int id)
        {
            _logger.LogInformation("delete medical record  called at {time}", DateTime.Now);

            var result = await _medicalRecordService.DeleteAsync(id);
            if (result == 0)
                return NotFound($"failed to delete medical record");

            return Ok("Deleted Successfully");
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Patient,Doctor")]

        public async Task<ActionResult<PatientMedicalRecordDto?>> GetById(int id)
        {
            _logger.LogInformation("get medical  record by id called at {time}", DateTime.Now);

            var dto = new GetMedicalRecordDto { MedicalRecordId = id };
            var record = await _medicalRecordService.GetByMedicalRecordIdAsync(dto);
            if (record == null) return NotFound();
            return Ok(record);
        }

        [HttpGet("by-doctor/{doctorId}")]
        [Authorize(Roles = "Patient,Doctor")]

        public async Task<ActionResult<List<PatientMedicalRecordDto>>> GetByDoctor(int doctorId)
        {
            _logger.LogInformation("get medical record by doctor id   called at {time}", DateTime.Now);

            var records = await _medicalRecordService.GetByDoctorId(doctorId);
            return Ok(records);
        }

        [HttpGet("by-patient/{patientId}")]
        [Authorize(Roles = "Patient,Doctor")]

        public async Task<ActionResult<List<PatientMedicalRecordDto>>> GetByPatient(int patientId)
        {
            _logger.LogInformation("get medical record by patient id called at {time}", DateTime.Now);

            var records = await _medicalRecordService.GetByPatientId(patientId);
            return Ok(records);
        }

        [HttpGet("records/doctor/{doctorId}/patient/{patientId}")]
        [Authorize(Roles = "Patient,Doctor")]

        public async Task<IActionResult> GetHistory(int doctorId, int patientId)
        {
            _logger.LogInformation("get medical record history by doctor id and patient id   called at {time}", DateTime.Now);

            var result = await _medicalRecordService.GetRecordsBetweenDoctorAndPatientAsync(doctorId, patientId);
            return Ok(result);
        }



    }
}
