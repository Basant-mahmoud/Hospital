using Hospital.Application.DTO.Specialization;
using Hospital.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecializationsController : ControllerBase
    {
        private readonly ISpecializationService _service;
        public SpecializationsController(ISpecializationService service)
        {
            _service = service;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<SpecializationDTO>> Create([FromBody] CreateSpecialization dto)
        {
            var result = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = result.SpecializationId }, result);
        }

        [HttpPost("add-from-excel")]
        public async Task<IActionResult> AddSpecializationsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var specializations = new List<CreateSpecialization>();

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
                        var dto = new CreateSpecialization
                        {
                            Name = sheet.Cells[row, 1].Text,
                            Description = sheet.Cells[row, 2].Text
                        };

                        var branches = sheet.Cells[row, 3].Text;
                        if (!string.IsNullOrEmpty(branches))
                        {
                            dto.BranchIds = branches
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(id => int.Parse(id.Trim()))
                                .ToList();
                        }

                        specializations.Add(dto);
                    }
                }
            }

            var results = new List<object>();
            foreach (var specialization in specializations)
            {
                try
                {
                    var created = await _service.AddAsync(specialization);

                    results.Add(new
                    {
                        specialization.Name,
                        Status = "Success",
                        SpecializationId = created.SpecializationId
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new
                    {
                        specialization.Name,
                        Status = "Failed",
                        Error = ex.Message
                    });
                }
            }

            return Ok(results);
        }

        [HttpGet("Get/{id}")]
        public async Task<ActionResult<SpecializationDTO>> Get(int id)
        {
            var result = await _service.GetAsync(id);
            return Ok(result);
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<SpecializationDTO>>> GetAll()
        {
            var result = await _service.GetAllSpecializaionInSystemAsync();
            return Ok(result);
        }

        [HttpPut("Update")]
        public async Task<ActionResult<SpecializationDTO>> Update([FromBody] UpdateSpecialization dto)
        {
            var updated = await _service.UpdateAsync(dto);
            return Ok("Updated Successfully");
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromBody] GetSpecializationDto dto)
        {
            var deletedRows = await _service.DeleteAsync(dto);
            if (deletedRows <= 0)
                return NotFound("Specialization not found or not deleted.");

            return Ok("Specialization deleted successfully.");
        }

        [HttpGet("Branch/{branchId}")]
        public async Task<ActionResult<IEnumerable<SpecializationDTO>>> GetAllByBranch(int branchId)
        {
            var result = await _service.GetAllByBranchAsync(branchId);
            return Ok(result);
        }


    }
}
