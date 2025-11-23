using AutoMapper;
using Hospital.Application.DTO.Banner;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BannersController : ControllerBase
    {
        private readonly IBannerService _bannerService;
        private readonly ILogger<BannersController> _logger;

        public BannersController(IBannerService bannerService, ILogger<BannersController> logger)
        {
            _bannerService = bannerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _bannerService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var banner = await _bannerService.GetByIdAsync(id);
            return banner == null ? NotFound() : Ok(banner);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBannerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _bannerService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.BannerId }, created);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateBannerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _bannerService.UpdateAsync(dto);
            return Ok("updated Successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _bannerService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return Ok("Deleted Successfully");
        }
    }
}
