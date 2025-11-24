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
        private readonly ILogger<DoctorController> _logger;

        public DoctorController(IDoctorService doctorService, ILogger<DoctorController> logger)
        {
            _doctorService = doctorService;
            _logger = logger;

        }

        // ------------------- Add Doctor -------------------
        [HttpPost("add")]
        public async Task<IActionResult> AddDoctor([FromBody] AddDoctorDto dto)
        {
            _logger.LogInformation("add doctor called at {time}", DateTime.Now);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _doctorService.AddAsync(dto);
            return Ok(created);
        }

        //  ------------ Add Doctor Using Excel Sheet--------
        [HttpPost("add-from-excel")]
        public async Task<IActionResult> AddDoctorsFromExcel(IFormFile file)
        {
            _logger.LogInformation("add doctor using excel called at {time}", DateTime.Now);

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var doctors = new List<AddDoctorDto>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

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
            _logger.LogInformation("Update doctor called at {time}", DateTime.Now);

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
            _logger.LogInformation("Delete doctor called at {time}", DateTime.Now);

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
            _logger.LogInformation("Get  doctor By Id called at {time}", DateTime.Now);

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
            _logger.LogInformation("get doctor by Branch called at {time}", DateTime.Now);

            var doctors = await _doctorService.GetAllAsync(branchId);
            return Ok(doctors);
        }

        // ------------------- Get All Doctors in System -------------------
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("get all doctor in system called at {time}", DateTime.Now);

            var doctors = await _doctorService.GetAllDoctorInSystemAsync();
            return Ok(doctors);
        }

        // ------------------- Get All Doctors in Specialization -------------------
        [HttpGet("BySpecialization/{specializationId}")]
        public async Task<IActionResult> GetDoctorsBySpecialization(int specializationId)
        {
            _logger.LogInformation("get doctor by Specialization  called at {time}", DateTime.Now);

            var result = await _doctorService.GetDoctorsBySpecializationIdAsync(specializationId);
            return Ok(result);
        }


        [HttpPut("self-update")]
        //[Authorize(Roles = "Doctor")]
        public async Task<IActionResult> SelfUpdate(DoctorSelfUpdateDto dto)
        {
            _logger.LogInformation("SelfUpdate doctor called at {time}", DateTime.Now);

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

        [HttpGet("{doctorId:int}/today")]
        public async Task<IActionResult> GetTodayAppointmentsForDoctor(int doctorId)
        {
            _logger.LogInformation("GetTodayAppointmentsForDoctor  called at {time}", DateTime.Now);

            var result = await _doctorService.GetTodayForDoctorAsync(doctorId);
            return Ok(result);
        }

        [HttpGet("{doctorId:int}/date/{date:datetime}")]
        public async Task<IActionResult> GetAppointmentsForDoctor(int doctorId, DateTime date)
        {
            _logger.LogInformation("GetAppointmentsForDoctor called at {time}", DateTime.Now);

            var dateOnly = DateOnly.FromDateTime(date);

            var result = await _doctorService.GetAppoinmentsForDoctorByDateAsync(doctorId, dateOnly);
            return Ok(result);
        }

        [HttpPut("CancelAppointment/doctor/{doctorId:int}/date/{date:datetime}")]
        public async Task<IActionResult> CancelAppointmentsForDoctorbyDate(int doctorId, DateTime date)
        {
            _logger.LogInformation("GetAppointmentsForDoctor called at {time}", DateTime.Now);

            var dateOnly = DateOnly.FromDateTime(date);

            var result = await _doctorService.CancelAppointmentsForDoctorbyDate(doctorId, dateOnly);
            if (result == 0)
            {
                _logger.LogInformation("No appointments found to cancel for doctorId: {doctorId} on date: {date}", doctorId, dateOnly);
                return NotFound("No appointments found to cancel.");
            }
            _logger.LogInformation("{result} appointment(s) cancelled for doctorId: {doctorId} on date: {date}", result, doctorId, dateOnly);
            return Ok($"{result} appointment(s) were successfully cancelled.");

        }


        [HttpPut("convertStatuesOFPaymentToPayied/{Appoimentid:int}")]
        public async Task<IActionResult> convertStatuesOFPaymentToPayied(int Appoimentid)
        {
            _logger.LogInformation("convertStatuesOFPaymentToPayied pay by cach  called at {time}", DateTime.Now);
            var result = await _doctorService.convertStatuesOFPaymentToPayied(Appoimentid);
            if(result==true)
               return Ok("Payment Paid successfully");
            return BadRequest("Payment Not Paid");
        }

    }
}
