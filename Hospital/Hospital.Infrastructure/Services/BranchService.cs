using AutoMapper;
using Hospital.Application.DTO.Branch;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _repository;
        private readonly IMapper _mapper;
        public BranchService(IBranchRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<BranchDto> CreateAsync(CreateBranchDto dto)
        {
            if (dto == null)
                throw new ValidationException("Request body cannot be null.");

            var existingBranches = await _repository.GetAllAsync();
            if (existingBranches.Any(b => b.BranchName.Equals(dto.BranchName, StringComparison.OrdinalIgnoreCase)))
                throw new ValidationException($"Branch with name '{dto.BranchName}' already exists.");

            var branch = _mapper.Map<Branch>(dto);
            branch.CreatedAt = DateTime.UtcNow;
            branch.UpdatedAt = DateTime.UtcNow;

            await _repository.AddAsync(branch);

            return _mapper.Map<BranchDto>(branch);
        }


        public async Task DeleteAsync(int id)
        {
            var branch = await _repository.GetByIdAsync(id);
            if (branch == null)
                throw new KeyNotFoundException($"Branch with ID {id} not found");

            await _repository.DeleteAsync(branch);
        }

        public async Task<IEnumerable<BranchDto>> GetAllAsync()
        {
            var branches = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<BranchDto>>(branches);
        }

        public async Task<BranchDto> GetByIdAsync(int id)
        {
            var branch = await _repository.GetByIdAsync(id);
            if (branch == null)
                throw new KeyNotFoundException($"Branch with ID {id} not found");

            return _mapper.Map<BranchDto>(branch);
        }

        public async Task UpdateAsync(UpdateBranchDto dto)
        {
            if (dto == null)
                throw new ValidationException("Request body cannot be null.");

            if (dto.BranchId <= 0)
                throw new ArgumentException("BranchId is required for update.");

            var existingBranch = await _repository.GetByIdAsync(dto.BranchId);
            if (existingBranch == null)
                throw new KeyNotFoundException($"Branch with ID {dto.BranchId} not found");

            var allBranches = await _repository.GetAllAsync();
            if (allBranches.Any(b =>
                    b.BranchId != dto.BranchId &&
                    b.BranchName.Equals(dto.BranchName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException($"Branch with name '{dto.BranchName}' already exists.");
            }

            _mapper.Map(dto, existingBranch);
            existingBranch.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existingBranch);
        }

    }
}
