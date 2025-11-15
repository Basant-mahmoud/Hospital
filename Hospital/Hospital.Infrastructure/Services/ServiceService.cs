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
        private readonly IBranchRepository _branchRepository;
        private readonly IMapper _mapper;

        public ServiceService(IServiceRepository serviceRepository, IMapper mapper, IBranchRepository branchRepository)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
            this._branchRepository = branchRepository;
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

            var branchIds = dto.BranchesID?.Select(b => b.BranchId).Distinct().ToList() ?? new List<int>();

            // Load branches
            var branches = new List<Branch>();
            if (branchIds.Any())
            {
                branches = await _branchRepository.GetByIdsAsync(branchIds);
                if (branches.Count != branchIds.Count)
                    throw new ArgumentException("One or more branches do not exist.");
            }

            // Check for duplicate service in these branches
            if (branchIds.Any())
            {
                var exists = await _serviceRepository.ExistsByNameInBranchesAsync(dto.Name, branchIds);
                if (exists)
                    throw new ArgumentException($"Service '{dto.Name}' already exists in one or more of the selected branches.");
            }

            var service = _mapper.Map<Service>(dto);
            service.Branches = branches;
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

            // Update branches
            if (dto.BranchesID != null)
            {
                var branchIds = dto.BranchesID.Select(b => b.BranchId).ToList();
                var branches = await _branchRepository.GetByIdsAsync(branchIds);
                existing.Branches = branches.ToList();
            }

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
