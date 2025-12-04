using Hospital.Application.DTO.Event;
using Hospital.Application.DTO.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Services
{
    public interface INewsService
    {
        Task<NewsDto> AddAsync(AddNewsDto newsdto);
        Task<int> UpdateAsync(NewsDto newsdto);
        Task<int> DeleteAsync(GetNewsDto newsdto);
        Task<NewsDto?> GetAsync(GetNewsDto newsdto);
        Task<IEnumerable<NewsDto>> GetAllAsync(int branchId);
        Task<IEnumerable<NewsDto>> GetAllNewsInSystemAsync();

    }
}
