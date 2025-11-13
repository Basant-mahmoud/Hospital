using Hospital.Application.DTO.Branch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Services
{
    public interface IBranchService
    {
        Task<IEnumerable<BranchDto>> GetAllAsync();
        Task<BranchDto> GetByIdAsync(int id);
        Task<BranchDto> CreateAsync(CreateBranchDto dto);
        Task UpdateAsync(UpdateBranchDto dto);
        Task DeleteAsync(int id);
    }
}
