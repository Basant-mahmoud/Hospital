using AutoMapper;
using Hospital.Application.DTO.Banner;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly IMapper _mapper;

        public BannerService(IBannerRepository bannerRepository, IMapper mapper)
        {
            _bannerRepository = bannerRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BannerDto>> GetAllAsync()
        {
            var banners = await _bannerRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BannerDto>>(banners);
        }

        public async Task<BannerDto?> GetByIdAsync(int id)
        {
            var banner = await _bannerRepository.GetByIdAsync(id);
            return banner == null ? null : _mapper.Map<BannerDto>(banner);
        }

        public async Task<BannerDto> CreateAsync(CreateBannerDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title cannot be empty.");

            var banner = _mapper.Map<Banner>(dto);
            banner.CreatedAt = DateTime.UtcNow;
            banner.UpdatedAt = DateTime.UtcNow;

            await _bannerRepository.AddAsync(banner);
            return _mapper.Map<BannerDto>(banner);
        }

        public async Task UpdateAsync(UpdateBannerDto dto)
        {
            var existing = await _bannerRepository.GetByIdAsync(dto.BannerId);
            if (existing == null)
                throw new KeyNotFoundException("Banner not found.");

            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            await _bannerRepository.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _bannerRepository.GetByIdAsync(id);
            if (entity == null)
                return false;

            await _bannerRepository.DeleteAsync(id);
            return true;
        }
    }
}
