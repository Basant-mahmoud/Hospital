using Hospital.Application.DTO.MedicalRecord;
using Hospital.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        [HttpPost]
        public async Task<ActionResult<PatientMedicalRecordDto>> Add([FromBody] AddMedicalRecordDto dto)
        {
            var result = await _medicalRecordService.AddAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult<int>> Update([FromBody] UpdateMedicalRecordDto dto)
        {
            var result = await _medicalRecordService.UpdateAsync(dto);
            return Ok(result);
        }

        // 3️⃣ Delete Medical Record
        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> Delete(int id)
        {
            var result = await _medicalRecordService.DeleteAsync(id);
            return Ok(result);
        }

        // 4️⃣ Get Medical Record by Id
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientMedicalRecordDto?>> GetById(int id)
        {
            var dto = new GetMedicalRecordDto { MedicalRecordId = id };
            var record = await _medicalRecordService.GetByMedicalRecordIdAsync(dto);
            if (record == null) return NotFound();
            return Ok(record);
        }

        // 5️⃣ Get all Medical Records by DoctorId
        [HttpGet("by-doctor/{doctorId}")]
        public async Task<ActionResult<List<PatientMedicalRecordDto>>> GetByDoctor(int doctorId)
        {
            var records = await _medicalRecordService.GetByDoctorId(doctorId);
            return Ok(records);
        }

        // 6️⃣ Get all Medical Records by PatientId
        [HttpGet("by-patient/{patientId}")]
        public async Task<ActionResult<List<PatientMedicalRecordDto>>> GetByPatient(int patientId)
        {
            var records = await _medicalRecordService.GetByPatientId(patientId);
            return Ok(records);
        }

       
        
    }
}
