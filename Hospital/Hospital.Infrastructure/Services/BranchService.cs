using AutoMapper;
using Hospital.Application.DTO.Branch;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            if (dto.BranchId <= 0 )
                throw new ArgumentException("BranchId is required for update.");

            var existing = await _repository.GetByIdAsync(dto.BranchId);
            if (existing == null)
                throw new KeyNotFoundException($"Branch with ID {dto.BranchId} not found");

            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existing);
        }
    }
}
