using AutoMapper;
using Hospital.Application.DTO.Branch;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<BranchService> _logger;

        public BranchService(IBranchRepository repository, IMapper mapper, ILogger<BranchService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BranchDto> CreateAsync(CreateBranchDto dto)
        {
            _logger.LogInformation("Creating new branch with name: {BranchName}", dto.BranchName);

            if (dto == null)
            {
                _logger.LogWarning("Failed to create branch: Request body is null");
                throw new ValidationException("Request body cannot be null.");
            }

            var existingBranches = await _repository.GetAllAsync();
            if (existingBranches.Any(b => b.BranchName.Equals(dto.BranchName, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("Failed to create branch: Branch with name '{BranchName}' already exists", dto.BranchName);
                throw new ValidationException($"Branch with name '{dto.BranchName}' already exists.");
            }

            var branch = _mapper.Map<Branch>(dto);
            branch.CreatedAt = DateTime.UtcNow;
            branch.UpdatedAt = DateTime.UtcNow;

            await _repository.AddAsync(branch);

            _logger.LogInformation("Branch created successfully with ID: {BranchId}", branch.BranchId);
            return _mapper.Map<BranchDto>(branch);
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting branch with ID: {BranchId}", id);

            var branch = await _repository.GetByIdAsync(id);
            if (branch == null)
            {
                _logger.LogWarning("Branch with ID {BranchId} not found", id);
                throw new KeyNotFoundException($"Branch with ID {id} not found");
            }

            await _repository.DeleteAsync(branch);
            _logger.LogInformation("Branch with ID {BranchId} deleted successfully", id);
        }

        public async Task<IEnumerable<BranchDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all branches");
            var branches = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<BranchDto>>(branches);
        }

        public async Task<BranchDto> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching branch by ID: {BranchId}", id);

            var branch = await _repository.GetByIdAsync(id);
            if (branch == null)
            {
                _logger.LogWarning("Branch with ID {BranchId} not found", id);
                throw new KeyNotFoundException($"Branch with ID {id} not found");
            }

            return _mapper.Map<BranchDto>(branch);
        }

        public async Task UpdateAsync(UpdateBranchDto dto)
        {
            _logger.LogInformation("Updating branch with ID: {BranchId}", dto.BranchId);

            if (dto == null)
            {
                _logger.LogWarning("Failed to update branch: Request body is null");
                throw new ValidationException("Request body cannot be null.");
            }

            if (dto.BranchId <= 0)
            {
                _logger.LogWarning("Failed to update branch: BranchId is invalid");
                throw new ArgumentException("BranchId is required for update.");
            }

            var existingBranch = await _repository.GetByIdAsync(dto.BranchId);
            if (existingBranch == null)
            {
                _logger.LogWarning("Branch with ID {BranchId} not found", dto.BranchId);
                throw new KeyNotFoundException($"Branch with ID {dto.BranchId} not found");
            }

            var allBranches = await _repository.GetAllAsync();
            if (allBranches.Any(b =>
                    b.BranchId != dto.BranchId &&
                    b.BranchName.Equals(dto.BranchName, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("Failed to update branch: Branch with name '{BranchName}' already exists", dto.BranchName);
                throw new ValidationException($"Branch with name '{dto.BranchName}' already exists.");
            }

            _mapper.Map(dto, existingBranch);
            existingBranch.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existingBranch);
            _logger.LogInformation("Branch with ID {BranchId} updated successfully", dto.BranchId);
        }
    }
}
