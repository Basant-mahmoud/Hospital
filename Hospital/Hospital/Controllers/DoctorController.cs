using Hospital.Application.DTO.Doctor;
using Hospital.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // ------------------- Add Doctor -------------------
        [HttpPost("add")]
        public async Task<IActionResult> AddDoctor([FromBody] AddDoctorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _doctorService.AddAsync(dto);
            return Ok(created);
        }

        // ------------------- Update Doctor -------------------
        [HttpPut("update")]
        public async Task<IActionResult> UpdateDoctor([FromBody] UpdateDoctorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _doctorService.UpdateAsync(dto);
            if (updated == 0)
                return NotFound(new { message = "Doctor not found" });

            return Ok(new { message = "Doctor updated successfully" });
        }

        // ------------------- Delete Doctor -------------------
        [HttpDelete("delete/{doctorId}")]
        public async Task<IActionResult> DeleteDoctor(int doctorId)
        {
            var dto = new GetDoctorDto { DoctorId = doctorId };
            var deleted = await _doctorService.DeleteAsync(dto);
            if (deleted == 0)
                return NotFound(new { message = "Doctor not found" });

            return Ok(new { message = "Doctor deleted successfully" });
        }

        // ------------------- Get Single Doctor -------------------
        [HttpGet("{doctorId}")]
        public async Task<IActionResult> GetDoctor(int doctorId)
        {
            var dto = new GetDoctorDto { DoctorId = doctorId };
            var doctor = await _doctorService.GetAsync(dto);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found" });

            return Ok(doctor);
        }

        // ------------------- Get All Doctors in Branch -------------------
        [HttpGet("branch/{branchId}")]
        public async Task<IActionResult> GetByBranch(int branchId)
        {
            var doctors = await _doctorService.GetAllAsync(branchId);
            return Ok(doctors);
        }

        // ------------------- Get All Doctors in System -------------------
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _doctorService.GetAllDoctorInSystemAsync();
            return Ok(doctors);
        }

        // ------------------- Get All Doctors in Specialization -------------------
        [HttpGet("BySpecialization/{specializationId}")]
        public async Task<IActionResult> GetDoctorsBySpecialization(int specializationId)
        {
            var result = await _doctorService.GetDoctorsBySpecializationIdAsync(specializationId);
            return Ok(result);
        }


        [HttpPut("doctor/self-update")]
        //[Authorize(Roles = "Doctor")]
        public async Task<IActionResult> SelfUpdate(DoctorSelfUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _doctorService.UpdatePersonalInfoAsync(dto);
                return Ok(new { Message = "Personal info updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }
    }
}
