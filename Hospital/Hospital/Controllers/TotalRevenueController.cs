using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
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
        public async Task<ActionResult<object>> GetDoctorRevenueInSpecificBranch(int doctorId, int branchId)
        {
            var revenue = await _revenueService.GetTotalRevenueByDoctorIdInSpecificBranchAsync(doctorId, branchId);
            return Ok(new { totalRevenue = revenue }); 
        }


        [HttpGet("doctor/{doctorId}/all-branches")]
        public async Task<ActionResult<decimal>> GetDoctorRevenueInAllBranches(int doctorId)
        {
            var revenue = await _revenueService.GetTotalRevenueByDoctorIdInAllBranchAsync(doctorId);
            return Ok(new { totalRevenue = revenue });
        }


        [HttpGet("all-branches")]
        public async Task<ActionResult<decimal>> GetTotalRevenueInAllBranches()
        {
            var revenue = await _revenueService.GetTotalRevenueInAllBranchAsync();
            return Ok(new { totalRevenue = revenue });
        }


        [HttpGet("branch/{branchId}")]
        public async Task<ActionResult<decimal>> GetTotalRevenueInSpecificBranch(int branchId)
        {
            var revenue = await _revenueService.GetTotalRevenueInSpecificBranchAllBranchAsync(branchId);
            return Ok(new { totalRevenue = revenue });
        }


        [HttpGet("month")]
        public async Task<ActionResult<object>> GetRevenueInSpecificMonth(int year, int month)
        {
            var revenue = await _revenueService.GetTotalRevenueInSpecificMonthAsync(year, month);
            return Ok(new { totalRevenue = revenue });
        }


        [HttpGet("year/{year}")]
        public async Task<ActionResult<object>> GetRevenueInSpecificYear(int year)
        {
            var revenue = await _revenueService.GetTotalRevenueInSpecificYearAsync(year);
            return Ok(new { totalRevenue = revenue });
        }


        [HttpGet("monthly-trend")]
        public async Task<IActionResult> GetMonthlyTrend(int year)
        {
            var data = await _revenueService.GetMonthlyRevenue(year);
            return Ok(new { data });
        }


        [HttpGet("branch-revenue")]
        public async Task<IActionResult> GetBranchRevenue()
        {
            var data = await _revenueService.GetRevenueByBranch();
            return Ok(new { data });
        }
    }
}
