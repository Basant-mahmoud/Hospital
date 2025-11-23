using Hospital.Application.DTO.Event;
using Hospital.Application.DTO.MedicalRecord;
using Hospital.Application.DTO.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedicalRecordDto = Hospital.Application.DTO.MedicalRecord.MedicalRecordDto;

namespace Hospital.Application.Interfaces.Services
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecordDto> AddAsync(AddMedicalRecordDto medicalRecord);
        Task<int> UpdateAsync(UpdateMedicalRecordDto medicalRecord);
        Task<int> DeleteAsync(int id);
        Task<MedicalRecordDto?> GetByMedicalRecordIdAsync(GetMedicalRecordDto dto);
        Task<List<MedicalRecordDto>> GetByDoctorId(int DoctorId);
        Task<List<MedicalRecordDto>> GetByPatientId(int PatientId);
        Task<List<MedicalRecordDto>> GetRecordsBetweenDoctorAndPatientAsync(int doctorId, int patientId);

    }
}
