using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Repos
{
    public interface ISpecializationRepository
    {
        Task<Specialization> AddAsync(Specialization specialization);
        Task<int> UpdateAsync(Specialization specialization);
        Task<int> DeleteAsync(Specialization specialization);
        Task<Specialization?> GetAsync(int id);
        Task<IEnumerable<Specialization>> GetAllByBranchAsync(int branchId);
        Task<IEnumerable<Specialization>> GetAllSpecializationInSystemAsync();
    }
}
