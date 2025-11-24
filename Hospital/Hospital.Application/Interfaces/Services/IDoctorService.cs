using Hospital.Application.DTO.Appointment;
using Hospital.Application.DTO.Doctor;
using Hospital.Application.DTO.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Services
{
    public interface IDoctorService
    {
        Task<DoctorDto> AddAsync(AddDoctorDto doctordto);
        Task<int> UpdateAsync(UpdateDoctorDto doctordto);
        Task<int> DeleteAsync(GetDoctorDto doctordto);
        Task<DoctorDto?> GetAsync(GetDoctorDto doctordto);
        Task<IEnumerable<DoctorDto>> GetAllAsync(int branchId);
        Task<IEnumerable<DoctorDto>> GetAllDoctorInSystemAsync();
        Task<IEnumerable<DoctorDto>> GetDoctorsBySpecializationIdAsync(int specializationId);
        Task UpdatePersonalInfoAsync(DoctorSelfUpdateDto dto);
        Task<List<AppoinmentandPaientDetaliesDto>> GetTodayForDoctorAsync(int doctorId);

        Task<List<AppoinmentandPaientDetaliesDto>> GetAppoinmentsForDoctorByDateAsync(int doctorId, DateOnly date);
        Task<bool> convertStatuesOFPaymentToPayied(int appoimentid);
        Task<int> CancelAppointmentsForDoctorbyDate(int id, DateOnly date);
    }

}
