using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.Interfaces.Repos
{
    public interface IMedicalRecordRepository
    {
        Task<MedicalRecord> AddAsync(MedicalRecord medical);
        Task<int> UpdateAsync(MedicalRecord medical);
        Task<int> DeleteAsync(MedicalRecord medical);
        Task<MedicalRecord?> GetAsync(int id);
        Task<IEnumerable<MedicalRecord>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId);

    }
}
