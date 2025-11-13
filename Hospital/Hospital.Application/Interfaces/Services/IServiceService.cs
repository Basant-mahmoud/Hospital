using Hospital.Application.DTO.ServiceDTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Services
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceDto>> GetAllAsync();
        Task<ServiceDto?> GetByIdAsync(int id);
        Task<ServiceDto> CreateAsync(CreateServiceDto dto);       
        Task UpdateAsync(UpdateServiceDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
