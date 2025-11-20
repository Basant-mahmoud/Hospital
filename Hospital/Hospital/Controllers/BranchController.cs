using Hospital.Application.DTO.Branch;
using Hospital.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;
        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }
        [HttpGet("GetAll")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetAll()
        {

            return Ok(await _branchService.GetAllAsync());
        }

        [HttpGet("GetBy/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _branchService.GetByIdAsync(id));
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateBranchDto dto)
        {
            var branch = await _branchService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = branch.BranchId }, branch);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateBranchDto dto)
        {
            await _branchService.UpdateAsync(dto);
            return Ok("Branch Updated Successfully");
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _branchService.DeleteAsync(id);
            return Ok("Branch deleted Successfully"); 
        }


    }
}
