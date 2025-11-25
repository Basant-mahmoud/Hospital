using Hospital.Application.DTO.Event;
using Hospital.Application.DTO.News;
using Hospital.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly ILogger<NewsController> _logger;

        public NewsController(INewsService newsService, ILogger<NewsController> logger)
        {
            _newsService = newsService;
            _logger = logger;
        }

        [HttpPost("CreateNews")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateNews([FromBody] AddNewsDto newsDto)
        {
            _logger.LogInformation("create News  called at {time}", DateTime.Now);

            var creatednews = await _newsService.AddAsync(newsDto);
            return CreatedAtAction(nameof(GetNews), new { id = creatednews.NewsId }, creatednews);
        }

        [HttpPost("GetNews")]

        public async Task<IActionResult> GetNews(GetNewsDto news)
        {
            _logger.LogInformation("get News  called at {time}", DateTime.Now);

            var newsDto = await _newsService.GetAsync(news);
            if (newsDto == null)
                return NotFound();

            return Ok(newsDto);
        }

        [HttpGet("GetAllNews")]
        public async Task<IActionResult> GetAllNews([FromQuery] int branchId)
        {
            _logger.LogInformation("get all News  to specific branch called at {time}", DateTime.Now);

            var news = await _newsService.GetAllAsync(branchId);
            if (news == null || !news.Any())
                return NotFound("No News found for this branch.");

            return Ok(news);
        }

        [HttpPut("UpdateNews")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UpdateEvent([FromBody] NewsDto news)
        {
            _logger.LogInformation("update News  called at {time}", DateTime.Now);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _newsService.UpdateAsync(news);
            if (result == 0)
                return NotFound($"No news found with ID = {news.NewsId}");

            return Ok("News updated successfully.");
        }

        [HttpDelete("DeleteNews")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteNews(GetNewsDto dto)
        {
            _logger.LogInformation("delete News  called at {time}", DateTime.Now);

            var result = await _newsService.DeleteAsync(dto);
            if (result == 0)
                return NotFound($"No news found with ID = {dto.NewsId}");

            return Ok($"News with ID = {dto.NewsId} deleted successfully.");
        }

        [HttpGet("GetAllNewsInSystem")]
        public async Task<IActionResult> GetAllNewsInSystem()
        {
            _logger.LogInformation("get all News in system  called at {time}", DateTime.Now);

            var news = await _newsService.GetAllEventInSystemAsync();
            if (news == null || !news.Any())
                return NotFound("No news found in the system.");
            return Ok(news);
        }
    }
}
