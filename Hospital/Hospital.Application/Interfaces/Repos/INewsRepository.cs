using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Repos
{
    public interface INewsRepository
    {
        Task<News> AddAsync(News news);
        Task<int> UpdateAsync(News news);
        Task<int> DeleteAsync(News news);
        Task<News?> GetAsync(int id);
        Task<IEnumerable<News>> GetAllAsync(int branchId);
        Task<IEnumerable<News>> GetAllNewsInSystemAsync();

    }
}
