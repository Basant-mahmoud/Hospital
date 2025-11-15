using Hospital.Application.DTO.Specialization;
using Hospital.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
