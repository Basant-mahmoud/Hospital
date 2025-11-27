using Hospital.Application.Interfaces.Services;
using Hospital.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TotalRevenueController : ControllerBase
    {
        private readonly ITotalRevenueService _revenueService;

        public TotalRevenueController(ITotalRevenueService revenueService)
        {
            _revenueService = revenueService;
        }

        [HttpGet("doctor/{doctorId}/branch/{branchId}")]
        public async Task<ActionResult<decimal>> GetDoctorRevenueInSpecificBranch(int doctorId, int branchId)
        {
            var revenue = await _revenueService.GetTotalRevenueByDoctorIdInSpecificBranchAsync(doctorId, branchId);
            return Ok(revenue);
        }

        [HttpGet("doctor/{doctorId}/all-branches")]
        public async Task<ActionResult<decimal>> GetDoctorRevenueInAllBranches(int doctorId)
        {
            var revenue = await _revenueService.GetTotalRevenueByDoctorIdInAllBranchAsync(doctorId);
            return Ok(revenue);
        }

        [HttpGet("all-branches")]
        public async Task<ActionResult<decimal>> GetTotalRevenueInAllBranches()
        {
            var revenue = await _revenueService.GetTotalRevenueInAllBranchAsync();
            return Ok(revenue);
        }

        [HttpGet("branch/{branchId}")]
        public async Task<ActionResult<decimal>> GetTotalRevenueInSpecificBranch(int branchId)
        {
            var revenue = await _revenueService.GetTotalRevenueInSpecificBranchAllBranchAsync(branchId);
            return Ok(revenue);
        }
    }
}
