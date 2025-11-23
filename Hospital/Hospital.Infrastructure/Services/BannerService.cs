using AutoMapper;
using Hospital.Application.DTO.Banner;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BannerService> _logger;

        public BannerService(IBannerRepository bannerRepository, IMapper mapper, ILogger<BannerService> logger)
        {
            _bannerRepository = bannerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BannerDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all banners");
            var banners = await _bannerRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BannerDto>>(banners);
        }

        public async Task<BannerDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching banner by ID: {BannerId}", id);
            var banner = await _bannerRepository.GetByIdAsync(id);
            if (banner == null)
            {
                _logger.LogWarning("Banner with ID {BannerId} not found", id);
                return null;
            }
            return _mapper.Map<BannerDto>(banner);
        }

        public async Task<BannerDto> CreateAsync(CreateBannerDto dto)
        {
            _logger.LogInformation("Creating new banner with title: {Title}", dto.Title);

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                _logger.LogWarning("Failed to create banner: Title is empty");
                throw new ArgumentException("Title cannot be empty.");
            }

            var banner = _mapper.Map<Banner>(dto);
            banner.CreatedAt = DateTime.UtcNow;
            banner.UpdatedAt = DateTime.UtcNow;

            await _bannerRepository.AddAsync(banner);

            _logger.LogInformation("Banner created successfully with ID: {BannerId}", banner.BannerId);
            return _mapper.Map<BannerDto>(banner);
        }

        public async Task UpdateAsync(UpdateBannerDto dto)
        {
            _logger.LogInformation("Updating banner with ID: {BannerId}", dto.BannerId);

            var existing = await _bannerRepository.GetByIdAsync(dto.BannerId);
            if (existing == null)
            {
                _logger.LogWarning("Banner with ID {BannerId} not found", dto.BannerId);
                throw new KeyNotFoundException("Banner not found.");
            }

            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            await _bannerRepository.UpdateAsync(existing);
            _logger.LogInformation("Banner with ID {BannerId} updated successfully", dto.BannerId);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting banner with ID: {BannerId}", id);

            var entity = await _bannerRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Banner with ID {BannerId} not found", id);
                return false;
            }

            await _bannerRepository.DeleteAsync(id);
            _logger.LogInformation("Banner with ID {BannerId} deleted successfully", id);
            return true;
        }
    }
}
