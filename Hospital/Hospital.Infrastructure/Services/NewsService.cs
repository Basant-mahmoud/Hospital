using AutoMapper;
using Hospital.Application.DTO.News;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
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
        public NewsService(INewsRepository newsRepository, IMapper mapper, IBranchService branchService)
        {
            _newsRepository = newsRepository;
            _branchService = branchService;
            _mapper = mapper;
        }

        public async Task<NewsDto> AddAsync(AddNewsDto newsdto)
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
            return _mapper.Map<NewsDto>(savedNews);
        }

        public async Task<int> DeleteAsync(GetNewsDto newsdto)
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

            return result;
        }

        public async Task<IEnumerable<NewsDto>> GetAllAsync(int branchId)
        {
            var newsList = await _newsRepository.GetAllAsync(branchId);
            if (newsList == null || !newsList.Any())
                return Enumerable.Empty<NewsDto>();

            return _mapper.Map<IEnumerable<NewsDto>>(newsList);
        }

        public async Task<IEnumerable<NewsDto>> GetAllEventInSystemAsync()
        {
            var newsList = await _newsRepository.GetAllEventInSystemAsync();
            if (newsList == null || !newsList.Any())
                return Enumerable.Empty<NewsDto>();

            return _mapper.Map<IEnumerable<NewsDto>>(newsList);
        }

        public async Task<NewsDto?> GetAsync(GetNewsDto newsdto)
        {
            if (newsdto.branchId <= 0)
                throw new ArgumentException("Invalid branch ID.", nameof(newsdto.branchId));

            var existingBranch = await _branchService.GetByIdAsync(newsdto.branchId);
            if (existingBranch == null)
                throw new KeyNotFoundException($"Branch with ID {newsdto.branchId} not found.");

            if (newsdto.NewsId <= 0)
                throw new ArgumentException("Invalid news ID.", nameof(newsdto.NewsId));

            var newsEntity = await _newsRepository.GetAsync(newsdto.NewsId);
            if (newsEntity == null)
                return null;

            return _mapper.Map<NewsDto>(newsEntity);
        }

        public async Task<int> UpdateAsync(NewsDto newsdto)
        {
            if (newsdto == null)
                throw new ArgumentNullException(nameof(newsdto), "News cannot be null.");

            if (string.IsNullOrWhiteSpace(newsdto.Title))
                throw new ArgumentException("News title cannot be empty.", nameof(newsdto.Title));

            if (newsdto.Date > DateTime.UtcNow)
                throw new ArgumentException("News date cannot be in the future.", nameof(newsdto.Date));

            if (newsdto.BranchId == null)
                throw new ArgumentException("Branch ID cannot be null.", nameof(newsdto.BranchId));

            var existingBranch = await _branchService.GetByIdAsync(newsdto.BranchId.Value);
            if (existingBranch == null)
                throw new KeyNotFoundException($"Branch with ID {newsdto.BranchId} not found.");

            var existingNews = await _newsRepository.GetAsync(newsdto.NewsId);
            if (existingNews == null)
                throw new KeyNotFoundException($"News with ID {newsdto.NewsId} not found.");

            _mapper.Map(newsdto, existingNews);
            existingNews.UpdatedAt = DateTime.UtcNow;

            return await _newsRepository.UpdateAsync(existingNews);
        }
    }
}
