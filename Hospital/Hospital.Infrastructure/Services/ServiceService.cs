using AutoMapper;
using Hospital.Application.DTO.ServiceDTOS;
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
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ServiceService> _logger;

        public ServiceService(IServiceRepository serviceRepository, IMapper mapper, IBranchRepository branchRepository, ILogger<ServiceService> logger)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
            _branchRepository = branchRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ServiceDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all services");
            var services = await _serviceRepository.GetAllAsync();
            _logger.LogInformation("{Count} services retrieved", services.Count());
            return _mapper.Map<IEnumerable<ServiceDto>>(services);
        }

        public async Task<ServiceDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching service with ID: {ServiceId}", id);
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
            {
                _logger.LogWarning("Service with ID {ServiceId} not found", id);
                return null;
            }

            _logger.LogInformation("Service with ID {ServiceId} retrieved successfully", id);
            return _mapper.Map<ServiceDto>(service);
        }

        public async Task<ServiceDto> CreateAsync(CreateServiceDto dto)
        {
            _logger.LogInformation("Creating service with Name: {ServiceName}", dto.Name);

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Service name cannot be empty.");

            var branchIds = dto.BranchesID?.Select(b => b.BranchId).Distinct().ToList() ?? new List<int>();
            var branches = new List<Branch>();

            if (branchIds.Any())
            {
                branches = await _branchRepository.GetByIdsAsync(branchIds);
                if (branches.Count != branchIds.Count)
                {
                    _logger.LogWarning("One or more branches do not exist for the new service");
                    throw new ArgumentException("One or more branches do not exist.");
                }

                var exists = await _serviceRepository.ExistsByNameInBranchesAsync(dto.Name, branchIds);
                if (exists)
                {
                    _logger.LogWarning("Service '{ServiceName}' already exists in one or more selected branches", dto.Name);
                    throw new ArgumentException($"Service '{dto.Name}' already exists in one or more of the selected branches.");
                }
            }

            var service = _mapper.Map<Service>(dto);
            service.Branches = branches;
            service.CreatedAt = DateTime.UtcNow;
            service.UpdatedAt = DateTime.UtcNow;

            await _serviceRepository.AddAsync(service);
            _logger.LogInformation("Service '{ServiceName}' created successfully", dto.Name);

            return _mapper.Map<ServiceDto>(service);
        }

        public async Task UpdateAsync(UpdateServiceDto dto)
        {
            _logger.LogInformation("Updating service with ID: {ServiceId}", dto.ServiceId);

            var existing = await _serviceRepository.GetByIdAsync(dto.ServiceId);
            if (existing == null)
            {
                _logger.LogWarning("Service with ID {ServiceId} not found", dto.ServiceId);
                throw new KeyNotFoundException("Service not found.");
            }

            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            if (dto.BranchesID != null)
            {
                var branchIds = dto.BranchesID.Select(b => b.BranchId).ToList();
                var branches = await _branchRepository.GetByIdsAsync(branchIds);
                existing.Branches = branches.ToList();
            }

            await _serviceRepository.UpdateAsync(existing);
            _logger.LogInformation("Service with ID {ServiceId} updated successfully", dto.ServiceId);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting service with ID: {ServiceId}", id);

            var entity = await _serviceRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Service with ID {ServiceId} not found", id);
                return false;
            }

            await _serviceRepository.DeleteAsync(id);
            _logger.LogInformation("Service with ID {ServiceId} deleted successfully", id);
            return true;
        }
    }
}
