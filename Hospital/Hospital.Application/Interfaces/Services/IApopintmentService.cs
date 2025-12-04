using Hospital.Application.DTO.Appointment;
using Hospital.Application.DTO.MedicalRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Services
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> AddAsync(AddAppointmentDto dto);
        Task<int> DeleteAsync(int id);
        Task<AppointmentDto?> GetByIdAsync(int id);
        Task<List<AppointmentDto>> GetByDoctorId(int DoctorId);
        Task<List<AppointmentDto>> GetByPatientId(int PatientId);
        Task<List<AppointmentDto>> GetAllAsync();
        Task<string> MarkAsCompletedAsync(int id);
        Task<List<AppointmentDto>> GetAllAppointmentCancelByDoctorId(int DoctorId);
        Task<AppoinmentandPaientDoctorDetaliesDto?> GetByIdwithdetailsofpatientanddoctorAsync(int id);
        Task<List<AppoinmentandPaientDetaliesDto>> GetAllCompletedAsync();
        Task<List<AppoinmentandPaientDetaliesDto>> GetAllCompletedForDoctorAsync(int doctorid);
    }
}
