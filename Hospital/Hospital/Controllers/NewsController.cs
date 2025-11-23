using Hospital.Application.DTO.Event;
using Hospital.Application.DTO.News;
using Hospital.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpPost("CreateNews")]
        public async Task<IActionResult> CreateEvent([FromBody] AddNewsDto newsDto)
        {
            var creatednews = await _newsService.AddAsync(newsDto);
            return CreatedAtAction(nameof(GetNews), new { id = creatednews.NewsId }, creatednews);
        }

        [HttpPost("GetNews")]
        public async Task<IActionResult> GetNews(GetNewsDto news)
        {
            var newsDto = await _newsService.GetAsync(news);
            if (newsDto == null)
                return NotFound();

            return Ok(newsDto);
        }

        [HttpGet("GetAllNews")]
        public async Task<IActionResult> GetAllNews([FromQuery] int branchId)
        {
            var news = await _newsService.GetAllAsync(branchId);
            if (news == null || !news.Any())
                return NotFound("No News found for this branch.");

            return Ok(news);
        }

        [HttpPut("UpdateNews")]
        public async Task<IActionResult> UpdateEvent([FromBody] NewsDto news)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _newsService.UpdateAsync(news);
            if (result == 0)
                return NotFound($"No news found with ID = {news.NewsId}");

            return Ok("News updated successfully.");
        }

        [HttpDelete("DeleteNews")]
        public async Task<IActionResult> DeleteNews(GetNewsDto dto)
        {
            var result = await _newsService.DeleteAsync(dto);
            if (result == 0)
                return NotFound($"No news found with ID = {dto.NewsId}");

            return Ok($"News with ID = {dto.NewsId} deleted successfully.");
        }

        [HttpGet("GetAllNewsInSystem")]
        public async Task<IActionResult> GetAllNewsInSystem()
        {
            var news = await _newsService.GetAllEventInSystemAsync();
            if (news == null || !news.Any())
                return NotFound("No news found in the system.");
            return Ok(news);
        }
    }
}
