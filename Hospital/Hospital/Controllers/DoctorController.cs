using Hospital.Application.DTO.Doctor;
using Hospital.Application.Interfaces.Services;
using Hospital.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

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





[HttpPost("add-from-excel")]
    public async Task<IActionResult> AddDoctorsFromExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var doctors = new List<AddDoctorDto>();

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            stream.Position = 0;

            // 🌟 EPPlus النسخة القديمة - الترخيص سهل
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.FirstOrDefault();
                if (sheet == null)
                    return BadRequest("Excel file has no sheets.");

                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                {
                    var dto = new AddDoctorDto
                    {
                        Name = sheet.Cells[row, 1].Text,
                        Email = sheet.Cells[row, 2].Text,
                        Username = sheet.Cells[row, 3].Text,
                        Password = sheet.Cells[row, 4].Text,
                        PhoneNumber = sheet.Cells[row, 5].Text,
                        SpecializationId = int.TryParse(sheet.Cells[row, 6].Text, out int spec) ? spec : 0,
                        ImageURL = sheet.Cells[row, 7].Text,
                        Biography = sheet.Cells[row, 9].Text,
                        ExperienceYears = int.TryParse(sheet.Cells[row, 10].Text, out int exp) ? exp : null,
                        ConsultationFees = decimal.TryParse(sheet.Cells[row, 11].Text, out decimal fees) ? fees : null,
                        Available = bool.TryParse(sheet.Cells[row, 12].Text, out bool avail) ? avail : null
                    };

                    // Branch IDs → comma separated
                    var branches = sheet.Cells[row, 8].Text;
                    if (!string.IsNullOrEmpty(branches))
                    {
                        dto.BranchIds = branches
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(id => int.Parse(id.Trim()))
                            .ToList();
                    }

                    doctors.Add(dto);
                }
            }
        }

        // ⬇ Process Each Doctor & Add To DB
        var results = new List<object>();

        foreach (var doctor in doctors)
        {
            try
            {
                var created = await _doctorService.AddAsync(doctor);

                results.Add(new
                {
                    doctor.Email,
                    Status = "Success",
                    DoctorId = created.DoctorId
                });
            }
            catch (Exception ex)
            {
                results.Add(new
                {
                    doctor.Email,
                    Status = "Failed",
                    Error = ex.Message
                });
            }
        }

        return Ok(results);
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
