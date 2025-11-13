using AutoMapper;
using Hospital.Application.DTO.ServiceDTOS;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Hospital.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper _mapper;

        public ServiceService(IServiceRepository serviceRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ServiceDto>> GetAllAsync()
        {
            var services = await _serviceRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ServiceDto>>(services);
        }

        public async Task<ServiceDto?> GetByIdAsync(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            return service == null ? null : _mapper.Map<ServiceDto>(service);
        }

        public async Task<ServiceDto> CreateAsync(CreateServiceDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Service name cannot be empty.");

            var service = _mapper.Map<Service>(dto);
            service.CreatedAt = DateTime.UtcNow;
            service.UpdatedAt = DateTime.UtcNow;

            await _serviceRepository.AddAsync(service);
            return _mapper.Map<ServiceDto>(service);
        }

        public async Task UpdateAsync(UpdateServiceDto dto)
        {
            var existing = await _serviceRepository.GetByIdAsync(dto.ServiceId);
            if (existing == null)
                throw new KeyNotFoundException("Service not found.");

            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            await _serviceRepository.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _serviceRepository.GetByIdAsync(id);
            if (entity == null)
                return false;
            await _serviceRepository.DeleteAsync(id);
            return true;
        }
    }
}
