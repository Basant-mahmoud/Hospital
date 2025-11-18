using Hospital.Application.DTO.Event;
using Hospital.Application.DTO.MedicalRecord;
using Hospital.Application.DTO.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Services
{
    public interface IMedicalRecordService
    {
        Task<PatientMedicalRecordDto> AddAsync(AddMedicalRecordDto @eventdto);
        Task<int> UpdateAsync(UpdateMedicalRecordDto @eventdto);
        Task<int> DeleteAsync(int id);
        Task<PatientMedicalRecordDto?> GetByMedicalRecordIdAsync(GetMedicalRecordDto dto);
        Task<List<PatientMedicalRecordDto>> GetByDoctorId(int DoctorId);
        Task<List<PatientMedicalRecordDto>> GetByPatientId(int PatientId);

    }
}
