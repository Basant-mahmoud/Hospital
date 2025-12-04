using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Services
{
    public interface ITotalRevenueService
    {
        Task<decimal> GetTotalRevenueByDoctorIdInSpecificBranchAsync(int doctorId, int branchId);
        Task<decimal> GetTotalRevenueByDoctorIdInAllBranchAsync(int doctorId);
        Task<decimal> GetTotalRevenueInAllBranchAsync();
        Task<decimal> GetTotalRevenueInSpecificBranchAllBranchAsync(int branchId);
        Task<decimal> GetTotalRevenueInSpecificYearAsync(int year);
        Task<decimal> GetTotalRevenueInSpecificMonthAsync(int year, int month);
        Task<List<object>> GetRevenueByBranch();
        Task<List<object>> GetMonthlyRevenue(int year);
    }
}
