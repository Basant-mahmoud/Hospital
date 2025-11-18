using AutoMapper;
using Hospital.Application.DTO.MedicalRecord;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
     public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _medicalRecordRepo;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private readonly IBranchService _branchService;
        private readonly IMapper _mapper;

        public MedicalRecordService(
            IMedicalRecordRepository medicalRecordRepo,
            IDoctorService doctorService,
            IPatientService patientService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IBranchService branchService)
        {
            _medicalRecordRepo = medicalRecordRepo;
            _doctorService = doctorService;
            _patientService = patientService;
            _mapper = mapper;
            _branchService = branchService;
        }


        public async Task<MedicalRecordDto> AddAsync(AddMedicalRecordDto dto)
        {
            // Check doctor
            var doctor = await _doctorService.GetAsync(new() { DoctorId = dto.DoctorId });
            if (doctor == null)
                throw new Exception($"Doctor with Id {dto.DoctorId} not found");

            // Check patient
            var patient = await _patientService.GetPatientByIdAsync(dto.PatientId);
            if (patient == null)
                throw new Exception($"Patient with Id {dto.PatientId} not found");

            // Check appointment
          //  var appointment = await _appointmentService.GetByIdAsync(dto.AppointmentId);
          //  if (appointment == null)
          //      throw new Exception($"Appointment with Id {dto.AppointmentId} not found");

            // Map
            var medicalRecord = _mapper.Map<MedicalRecord>(dto);
            medicalRecord.CreatedAt = DateTime.UtcNow;
            medicalRecord.UpdatedAt = DateTime.UtcNow;

            // Save
            await _medicalRecordRepo.AddAsync(medicalRecord);

            // Reload with full relations
            var fullRecord = await _medicalRecordRepo.GetByIdWithRelationsAsync(medicalRecord.RecordId);

            // Map result
            return _mapper.Map<MedicalRecordDto>(fullRecord);
        }


        // 2️⃣ Delete
        public async Task<int> DeleteAsync(int id)
        {
            var record = await _medicalRecordRepo.GetAsync(id);
            if (record == null) throw new Exception($"MedicalRecord with Id {id} not found");
            return await _medicalRecordRepo.DeleteAsync(record);
        }

      
        // 3️⃣ Get one record
        public async Task<MedicalRecordDto?> GetByMedicalRecordIdAsync(GetMedicalRecordDto dto)
        {

            var record = await _medicalRecordRepo.GetAsync(dto.MedicalRecordId);
            if (record == null) return null;
            return _mapper.Map<MedicalRecordDto>(record);
        }

       
        // 6️⃣ Update record
        public async Task<int> UpdateAsync(UpdateMedicalRecordDto dto)
        {
            var record = await _medicalRecordRepo.GetAsync(dto.RecordId);
            if (record == null) throw new Exception($"MedicalRecord with Id {dto.RecordId} not found");

            _mapper.Map(dto, record); 
            record.UpdatedAt = DateTime.UtcNow;

            return await _medicalRecordRepo.UpdateAsync(record);
        }
        public async Task<List<MedicalRecordDto>> GetByDoctorId(int doctorId)
        {
            var records = await _medicalRecordRepo.GetByDoctorIdAsync(doctorId);
            return _mapper.Map<List<MedicalRecordDto>>(records);
        }

        public async Task<List<MedicalRecordDto>> GetByPatientId(int patientId)
        {
            var records = await _medicalRecordRepo.GetByPatientIdAsync(patientId);
            return _mapper.Map<List<MedicalRecordDto>>(records);
        }

    }
}
