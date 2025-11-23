using AutoMapper;
using Hospital.Application.DTO.News;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly IBranchService _branchService;
        private readonly IMapper _mapper;
        private readonly ILogger<NewsService> _logger;

        public NewsService(INewsRepository newsRepository, IMapper mapper, IBranchService branchService, ILogger<NewsService> logger)
        {
            _newsRepository = newsRepository;
            _branchService = branchService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<NewsDto> AddAsync(AddNewsDto newsdto)
        {
            _logger.LogInformation("Adding a new news item: {Title}", newsdto.Title);

            try
            {
                if (string.IsNullOrWhiteSpace(newsdto.Title))
                    throw new ArgumentException("News title cannot be empty.", nameof(newsdto.Title));

                if (newsdto.BranchId == null)
                    throw new ArgumentException("Branch ID cannot be null.", nameof(newsdto.BranchId));

                if (newsdto.Date > DateTime.UtcNow)
                    throw new ArgumentException("News date cannot be in the future.", nameof(newsdto.Date));

                if (string.IsNullOrWhiteSpace(newsdto.ImageURL) ||
                    !Uri.IsWellFormedUriString(newsdto.ImageURL, UriKind.Absolute))
                    throw new ArgumentException("Image URL is invalid.", nameof(newsdto.ImageURL));

                var existingBranch = await _branchService.GetByIdAsync(newsdto.BranchId.Value);
                if (existingBranch == null)
                    throw new KeyNotFoundException($"Branch with ID {newsdto.BranchId} not found.");

                var news = _mapper.Map<News>(newsdto);
                news.CreatedAt = DateTime.UtcNow;
                news.UpdatedAt = DateTime.UtcNow;

                var savedNews = await _newsRepository.AddAsync(news);
                _logger.LogInformation("News item added successfully with ID: {NewsId}", savedNews.NewsId);

                return _mapper.Map<NewsDto>(savedNews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding news: {Title}", newsdto.Title);
                throw;
            }
        }

        public async Task<int> DeleteAsync(GetNewsDto newsdto)
        {
            _logger.LogInformation("Deleting news item with ID: {NewsId}", newsdto.NewsId);

            try
            {
                if (newsdto == null)
                    throw new ArgumentNullException(nameof(newsdto), "News cannot be null.");

                if (newsdto.NewsId <= 0)
                    throw new ArgumentException("Invalid news ID.", nameof(newsdto.NewsId));

                if (newsdto.branchId <= 0)
                    throw new ArgumentException("Invalid branch ID.", nameof(newsdto.branchId));

                var existingBranch = await _branchService.GetByIdAsync(newsdto.branchId);
                if (existingBranch == null)
                    throw new KeyNotFoundException($"Branch with ID {newsdto.branchId} not found.");

                var newsEntity = await _newsRepository.GetAsync(newsdto.NewsId);
                if (newsEntity == null)
                    throw new KeyNotFoundException($"News with ID {newsdto.NewsId} not found.");

                var result = await _newsRepository.DeleteAsync(newsEntity);
                if (result == 0)
                    throw new Exception("Failed to delete the news.");

                _logger.LogInformation("News item deleted successfully with ID: {NewsId}", newsdto.NewsId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting news with ID: {NewsId}", newsdto.NewsId);
                throw;
            }
        }

        public async Task<IEnumerable<NewsDto>> GetAllAsync(int branchId)
        {
            _logger.LogInformation("Retrieving all news for branch ID: {BranchId}", branchId);
            var newsList = await _newsRepository.GetAllAsync(branchId);

            if (newsList == null || !newsList.Any())
            {
                _logger.LogInformation("No news found for branch ID: {BranchId}", branchId);
                return Enumerable.Empty<NewsDto>();
            }

            return _mapper.Map<IEnumerable<NewsDto>>(newsList);
        }

        public async Task<IEnumerable<NewsDto>> GetAllEventInSystemAsync()
        {
            _logger.LogInformation("Retrieving all news in the system");
            var newsList = await _newsRepository.GetAllEventInSystemAsync();

            if (newsList == null || !newsList.Any())
            {
                _logger.LogInformation("No news found in the system");
                return Enumerable.Empty<NewsDto>();
            }

            return _mapper.Map<IEnumerable<NewsDto>>(newsList);
        }

        public async Task<NewsDto?> GetAsync(GetNewsDto newsdto)
        {
            _logger.LogInformation("Getting news with ID: {NewsId} for branch ID: {BranchId}", newsdto.NewsId, newsdto.branchId);

            var existingBranch = await _branchService.GetByIdAsync(newsdto.branchId);
            if (existingBranch == null)
                throw new KeyNotFoundException($"Branch with ID {newsdto.branchId} not found.");

            var newsEntity = await _newsRepository.GetAsync(newsdto.NewsId);
            if (newsEntity == null)
            {
                _logger.LogWarning("News with ID {NewsId} not found", newsdto.NewsId);
                return null;
            }

            return _mapper.Map<NewsDto>(newsEntity);
        }

        public async Task<int> UpdateAsync(NewsDto newsdto)
        {
            _logger.LogInformation("Updating news with ID: {NewsId}", newsdto.NewsId);

            try
            {
                if (newsdto == null)
                    throw new ArgumentNullException(nameof(newsdto), "News cannot be null.");

                var existingBranch = await _branchService.GetByIdAsync(newsdto.BranchId.Value);
                if (existingBranch == null)
                    throw new KeyNotFoundException($"Branch with ID {newsdto.BranchId} not found.");

                var existingNews = await _newsRepository.GetAsync(newsdto.NewsId);
                if (existingNews == null)
                    throw new KeyNotFoundException($"News with ID {newsdto.NewsId} not found.");

                _mapper.Map(newsdto, existingNews);
                existingNews.UpdatedAt = DateTime.UtcNow;

                var result = await _newsRepository.UpdateAsync(existingNews);
                _logger.LogInformation("News updated successfully with ID: {NewsId}", newsdto.NewsId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating news with ID: {NewsId}", newsdto.NewsId);
                throw;
            }
        }
    }
}
