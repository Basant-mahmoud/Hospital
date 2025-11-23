using AutoMapper;
using Hospital.Application.DTO.Specialization;
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
    public class SpecializationService : ISpecializationService
    {
        private readonly ISpecializationRepository _specRepo;
        private readonly IBranchRepository _branchRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<SpecializationService> _logger;

        public SpecializationService(ISpecializationRepository specRepo, IBranchRepository branchRepo, IMapper mapper, ILogger<SpecializationService> logger)
        {
            _specRepo = specRepo;
            _branchRepo = branchRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SpecializationDTO> AddAsync(CreateSpecialization dto)
        {
            _logger.LogInformation("Adding specialization: {SpecializationName}", dto.Name);

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Specialization name is required.");

            if (dto.BranchIds == null || !dto.BranchIds.Any())
                throw new ArgumentException("At least one branch must be assigned.");

            var branches = await _branchRepo.GetBranchesByIdsAsync(dto.BranchIds);
            if (branches.Count != dto.BranchIds.Count)
                throw new ArgumentException("One or more branches not found.");

            var existingSpecs = await _specRepo.GetAllSpecializationInSystemAsync();
            foreach (var branch in branches)
            {
                if (existingSpecs.Any(s => s.Name.Trim().ToLower() == dto.Name.Trim().ToLower()
                                           && s.Branches.Any(b => b.BranchId == branch.BranchId)))
                {
                    _logger.LogWarning("Specialization '{SpecializationName}' already exists in branch '{BranchName}'", dto.Name, branch.BranchName);
                    throw new InvalidOperationException(
                        $"Specialization '{dto.Name}' already exists in branch '{branch.BranchName}'.");
                }
            }

            var specialization = _mapper.Map<Specialization>(dto);
            specialization.CreatedAt = DateTime.UtcNow;
            specialization.UpdatedAt = DateTime.UtcNow;
            specialization.Branches = branches;

            await _specRepo.AddAsync(specialization);
            _logger.LogInformation("Specialization '{SpecializationName}' added successfully", dto.Name);

            return _mapper.Map<SpecializationDTO>(specialization);
        }

        public async Task<int> DeleteAsync(GetSpecializationDto dto)
        {
            _logger.LogInformation("Deleting specialization with ID {SpecializationId} from branch {BranchId}", dto.SpecializationId, dto.BranchId);

            var specialization = await _specRepo.GetAsync(dto.SpecializationId);
            if (specialization == null)
            {
                _logger.LogWarning("Specialization with ID {SpecializationId} not found", dto.SpecializationId);
                throw new KeyNotFoundException("Specialization not found.");
            }

            var branch = specialization.Branches.FirstOrDefault(b => b.BranchId == dto.BranchId);
            if (branch == null)
            {
                _logger.LogWarning("Specialization {SpecializationId} does not belong to branch {BranchId}", dto.SpecializationId, dto.BranchId);
                throw new KeyNotFoundException("Specialization does not belong to this branch.");
            }

            specialization.Branches.Remove(branch);
            var result = await _specRepo.UpdateAsync(specialization);
            _logger.LogInformation("Specialization {SpecializationId} removed from branch {BranchId}", dto.SpecializationId, dto.BranchId);
            return result;
        }

        public async Task<IEnumerable<SpecializationInfoDto>> GetAllByBranchAsync(int branchId)
        {
            _logger.LogInformation("Fetching all specializations for branch {BranchId}", branchId);
            var specializations = await _specRepo.GetAllByBranchAsync(branchId);
            return _mapper.Map<IEnumerable<SpecializationInfoDto>>(specializations);
        }

        public async Task<IEnumerable<SpecializationDTO>> GetAllSpecializaionInSystemAsync()
        {
            _logger.LogInformation("Fetching all specializations in the system");
            var entities = await _specRepo.GetAllSpecializationInSystemAsync();
            return _mapper.Map<IEnumerable<SpecializationDTO>>(entities);
        }

        public async Task<SpecializationDTO?> GetAsync(int id)
        {
            _logger.LogInformation("Fetching specialization with ID {SpecializationId}", id);
            var entity = await _specRepo.GetAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Specialization with ID {SpecializationId} not found", id);
                return null;
            }
            return _mapper.Map<SpecializationDTO>(entity);
        }

        public async Task<int> UpdateAsync(UpdateSpecialization dto)
        {
            _logger.LogInformation("Updating specialization with ID {SpecializationId}", dto.SpecializationId);

            var specialization = await _specRepo.GetAsync(dto.SpecializationId);
            if (specialization == null)
            {
                _logger.LogWarning("Specialization with ID {SpecializationId} not found", dto.SpecializationId);
                throw new KeyNotFoundException("Specialization not found.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Specialization name is required.");

            var existingSpecs = await _specRepo.GetAllSpecializationInSystemAsync();

            // Check for duplicates in other branches
            foreach (var branchId in dto.BranchIds)
            {
                if (existingSpecs.Any(s =>
                    s.SpecializationId != dto.SpecializationId &&
                    s.Name.Trim().ToLower() == dto.Name.Trim().ToLower() &&
                    s.Branches.Any(b => b.BranchId == branchId)))
                {
                    _logger.LogWarning("Specialization '{SpecializationName}' already exists in branch {BranchId}", dto.Name, branchId);
                    throw new InvalidOperationException(
                        $"Specialization '{dto.Name}' already exists in branch '{branchId}'.");
                }
            }

            var branches = await _branchRepo.GetBranchesByIdsAsync(dto.BranchIds);
            if (branches.Count != dto.BranchIds.Count)
            {
                var missingBranchIds = dto.BranchIds.Except(branches.Select(b => b.BranchId));
                _logger.LogWarning("Branches with IDs {MissingBranchIds} not found", string.Join(", ", missingBranchIds));
                throw new KeyNotFoundException($"Branches with IDs {string.Join(", ", missingBranchIds)} not found.");
            }

            specialization.Name = dto.Name;
            specialization.Description = dto.Description;
            specialization.UpdatedAt = DateTime.UtcNow;

            // Remove branches that are no longer selected
            var branchesToRemove = specialization.Branches.Where(b => !dto.BranchIds.Contains(b.BranchId)).ToList();
            foreach (var branch in branchesToRemove)
                specialization.Branches.Remove(branch);

            // Add new branches that are not already assigned
            foreach (var branch in branches)
            {
                if (!specialization.Branches.Any(b => b.BranchId == branch.BranchId))
                    specialization.Branches.Add(branch);
            }

            var result = await _specRepo.UpdateAsync(specialization);
            _logger.LogInformation("Specialization {SpecializationId} updated successfully", dto.SpecializationId);
            return result;
        }
    }
}
