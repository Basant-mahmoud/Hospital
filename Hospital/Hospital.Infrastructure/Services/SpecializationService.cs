using AutoMapper;
using Hospital.Application.DTO.Specialization;
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
    public class SpecializationService : ISpecializationService
    {
        private readonly ISpecializationRepository _specRepo;
        private readonly IBranchRepository _branchRepo;
        private readonly IMapper _mapper;

        public SpecializationService(
            ISpecializationRepository specRepo,
            IBranchRepository branchRepo,
            IMapper mapper)
        {
            _specRepo = specRepo;
            _branchRepo = branchRepo;
            _mapper = mapper;
        }
        public async Task<SpecializationDTO> AddAsync(CreateSpecialization dto)
        {
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
                    throw new InvalidOperationException(
                        $"Specialization '{dto.Name}' already exists in branch '{branch.BranchName}'.");
                }
            }

            var specialization = _mapper.Map<Specialization>(dto);
            specialization.CreatedAt = DateTime.UtcNow;
            specialization.UpdatedAt = DateTime.UtcNow;
            specialization.Branches = branches;

            await _specRepo.AddAsync(specialization);
            return _mapper.Map<SpecializationDTO>(specialization);
        }

        public async Task<int> DeleteAsync(GetSpecializationDto dto)
        {
            if(dto.BranchId == null)
                throw new ArgumentNullException("BranchId must exist");
            var branch = await _branchRepo.GetByIdAsync(dto.BranchId);
            if (branch == null)
                throw new KeyNotFoundException("Branch not found.");
            var specialization = await _specRepo.GetAsync(dto.SpecializationId);
            if (specialization == null)
                throw new KeyNotFoundException("Specialization not found.");

            return await _specRepo.DeleteAsync(specialization);
        }

        public async Task<IEnumerable<SpecializationInfoDto>> GetAllByBranchAsync(int branchId)
        {
            var specializations = await _specRepo.GetAllByBranchAsync(branchId);
            var specializationDtos = _mapper.Map<IEnumerable<SpecializationInfoDto>>(specializations);

            return specializationDtos;
        }

        public async Task<IEnumerable<SpecializationDTO>> GetAllSpecializaionInSystemAsync()
        {
            var entities = await _specRepo.GetAllSpecializationInSystemAsync();
            return _mapper.Map<IEnumerable<SpecializationDTO>>(entities);
        }

        public async Task<SpecializationDTO?> GetAsync(int id)
        {
            var entity = await _specRepo.GetAsync(id);
            return _mapper.Map<SpecializationDTO>(entity);
        }

        public async Task<int> UpdateAsync(UpdateSpecialization dto)
        {
            var specialization = await _specRepo.GetAsync(dto.SpecializationId);
            if (specialization == null)
                throw new KeyNotFoundException("Specialization not found.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Specialization name is required.");

            // Check for duplicates in other branches
            var existingSpecs = await _specRepo.GetAllSpecializationInSystemAsync();
            foreach (var branchId in dto.BranchIds)
            {
                if (existingSpecs.Any(s =>
                    s.SpecializationId != dto.SpecializationId &&
                    s.Name.Trim().ToLower() == dto.Name.Trim().ToLower() &&
                    s.Branches.Any(b => b.BranchId == branchId)))
                {
                    var branch = await _branchRepo.GetByIdAsync(branchId);
                    throw new InvalidOperationException(
                        $"Specialization '{dto.Name}' already exists in branch '{branch?.BranchName}'.");
                }
            }

            var branches = await _branchRepo.GetBranchesByIdsAsync(dto.BranchIds);
            // Fail if any branch is missing
            if (branches.Count != dto.BranchIds.Count)
            {
                var missingBranchIds = dto.BranchIds.Except(branches.Select(b => b.BranchId));
                throw new KeyNotFoundException($"Branches with IDs {string.Join(", ", missingBranchIds)} not found.");
            }

            specialization.Name = dto.Name;
            specialization.Description = dto.Description;
            specialization.UpdatedAt = DateTime.UtcNow;

            // Add only branches not already assigned
            foreach (var branch in branches)
            {
                if (!specialization.Branches.Any(b => b.BranchId == branch.BranchId))
                    specialization.Branches.Add(branch);
            }
            return await _specRepo.UpdateAsync(specialization);
        }
    }
}
