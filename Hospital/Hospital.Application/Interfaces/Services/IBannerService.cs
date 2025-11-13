using Hospital.Application.DTO.Banner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Services
{
    public interface IBannerService
    {
        Task<IEnumerable<BannerDto>> GetAllAsync();
        Task<BannerDto?> GetByIdAsync(int id);
        Task<BannerDto> CreateAsync(CreateBannerDto dto);     
        Task UpdateAsync(UpdateBannerDto dto);                  
        Task<bool> DeleteAsync(int id);
    }
}
