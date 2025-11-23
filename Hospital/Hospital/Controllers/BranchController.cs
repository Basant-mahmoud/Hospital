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
        private readonly ILogger<BranchController> _logger;

        public BranchController(IBranchService branchService, ILogger<BranchController> logger)
        {
            _branchService = branchService;
            _logger = logger;
        }
        [HttpGet("GetAll")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Get All Branch called at {time}", DateTime.Now);


            return Ok(await _branchService.GetAllAsync());
        }

        [HttpGet("GetBy/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Get Branch By Id called at {time}", DateTime.Now);

            return Ok(await _branchService.GetByIdAsync(id));
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateBranchDto dto)
        {
            _logger.LogInformation("Create Branch called at {time}", DateTime.Now);

            var branch = await _branchService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = branch.BranchId }, branch);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateBranchDto dto)
        {
            _logger.LogInformation("Update Branch called at {time}", DateTime.Now);

            await _branchService.UpdateAsync(dto);
            return Ok("Branch Updated Successfully");
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Delete Branch called at {time}", DateTime.Now);

            await _branchService.DeleteAsync(id);
            return Ok("Branch deleted Successfully"); 
        }


    }
}
